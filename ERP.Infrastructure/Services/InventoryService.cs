using ERP.Application.Interfaces;
using ERP.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ERP.Application.DTOs;
using ERP.Application.Interfaces;
using ERP.Domain.Entities;
using ERP.Domain.Enums;
using ERP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ERP.Infrastructure.Services
{
    /*
 * InventoryService = 庫存服務（實作）
 * 這裡會：
 * 1) 驗證（Qty>0，商品/倉庫存在）
 * 2) 寫 StockLedger（台帳：每一筆異動）
 * 3) 更新 InventoryBalance（結餘：目前庫存）
 * 4) Transaction：確保台帳與結餘一致
 */
    public class InventoryService : IInventoryService
    {
        private readonly AppDbContext _db;
        public InventoryService(AppDbContext db)
        {
            _db = db;
        }

        //入庫（庫存台帳） 更新等
        public async Task<StockInResponse> StockInAsync(StockInRequest req, CancellationToken ct = default)
        {
            // (A) 基本防呆/驗證
            // 入庫數量一定要 > 0（入庫用正數；出庫會在下一步做）
            if (req.Qty <= 0)
            {
                throw new InvalidOperationException("入庫數量 Qty 必須大於 0。");
            }
            // 商品必須存在
            var productExists = await _db.Products.AnyAsync(p => p.Id == req.ProductId, ct);
            if (!productExists) { throw new InvalidOperationException("找不到商品（ProductId）。"); }
            // 倉庫必須存在
            var warehouseExists = await _db.Warehouses.AnyAsync(w => w.Id == req.WarehouseId, ct);
            if (!warehouseExists) { throw new InvalidOperationException("找不到倉庫（WarehouseId）。"); }
            //單據資訊清洗：去空白，避免資料不乾淨
            var refType = req.RefType.Trim();
            var refNo = req.RefNo.Trim();

            // (B) Transaction 交易一致性 開啟
            // 這非常重要：
            // 台帳寫成功但結餘沒更新 > 庫存會亂
            // 結餘更新成功但台帳沒寫 > 查歷史會缺資料
            // 所以要「同生共死」：要嘛一起成功，要嘛一起失敗。
            await using var tx = await _db.Database.BeginTransactionAsync(ct);//接下來的資料庫操作，要嘛全部成功 要嘛全部失敗回滾（Rollback）

            // (C)1) 寫入 StockLedger（庫存台帳）
            // 台帳 = 每一筆庫存異動紀錄（可追溯、可稽核）
            var ledger = new StockLedger
            {
                ProductId = req.ProductId,
                WarehouseId = req.WarehouseId,
                // 這次異動是「採購入庫」
                TxnType = StockTxnType.PurchaseReceipt,
                // 入庫：數量為正數
                Qty = req.Qty,
                TxnAtUtc = DateTime.UtcNow,
                RefType = refType,
                RefNo = refNo,
                Remark = req.Remark

            };
            _db.StockLedgers.Add(ledger);

            // (D) 2) 更新 InventoryBalance（庫存結餘）
            // 結餘 = 目前庫存（商品×倉庫）
            // 找得到就 +Qty
            // 找不到就新增（代表第一次入這個倉）
            var balance = await _db.InventoryBalances.SingleOrDefaultAsync(x => x.ProductId == req.ProductId && x.WarehouseId == req.WarehouseId, ct);
            //第一次入倉
            if (balance == null)
            {
                balance = new InventoryBalance
                {
                    ProductId = req.ProductId,
                    WarehouseId = req.WarehouseId,
                    OnHandQty = 0,
                    UpdatedAtUtc = DateTime.UtcNow
                };

                _db.InventoryBalances.Add(balance);
            }

            // 入庫：庫存增加
            balance.OnHandQty += req.Qty;
            balance.UpdatedAtUtc = DateTime.UtcNow;

            // (E) 儲存變更並提交 Transaction
            await _db.SaveChangesAsync(ct);
            // Commit：交易提交（到這行才算真的完成）
            await tx.CommitAsync(ct);

            // (F) 回傳結果
            return new StockInResponse(
            LedgerId: ledger.Id,
            ProductId: ledger.ProductId,
            WarehouseId: ledger.WarehouseId,
            Qty: ledger.Qty,
            TxnAtUtc: ledger.TxnAtUtc,
            RefType: ledger.RefType,
            RefNo: ledger.RefNo
        );
        }


        //出庫
        public async Task<StockOutResponse> StockOutAsync(StockOutRequest req, CancellationToken ct = default)
        {
            // (A) 防呆：出庫 Qty 必須 > 0
            if (req.Qty <= 0) { throw new InvalidOperationException("出庫數量 Qty 必須大於 0。"); }

            //清洗單據資訊(去頭尾空白)
            var refType = req.RefType.Trim();
            var refNo = req.RefNo.Trim();

            //(B) 先找結餘 InventoryBalance /如果為空拋出異常
            var balance = await _db.InventoryBalances.SingleOrDefaultAsync(x => x.ProductId == req.ProductId && x.WarehouseId == req.WarehouseId, ct);
            if (balance == null) { throw new InvalidOperationException("該倉庫尚無此商品庫存（找不到結餘資料）。"); }

            // (C) 核心規則：不能扣成負數
            if (balance.OnHandQty < req.Qty) { throw new InvalidOperationException($"庫存不足：現有 {balance.OnHandQty}，欲出庫 {req.Qty}。"); }

            //(D) Transaction：台帳 + 結餘要一起成功(交易功能打開)
            await using var tx = await _db.Database.BeginTransactionAsync(ct);

            // 1) 寫台帳（出庫寫「負數」）
            var ledger = new StockLedger
            {
                ProductId = req.ProductId,
                WarehouseId = req.WarehouseId,
                TxnType = StockTxnType.SalesShipment, // 銷售出貨
                Qty = -req.Qty,                       //  出庫寫負數
                TxnAtUtc = DateTime.UtcNow,
                RefType = refType,
                RefNo = refNo,
                Remark = req.Remark
            };
            _db.StockLedgers.Add(ledger);

            // 2) 更新結餘（扣庫存）
            balance.OnHandQty -= req.Qty;
            balance.UpdatedAtUtc = DateTime.UtcNow;

            await _db.SaveChangesAsync(ct);
            await tx.CommitAsync(ct);

            return new StockOutResponse
            (
               LedgerId: ledger.Id,
               ProductId: ledger.ProductId,
               WarehouseId: ledger.WarehouseId,
               Qty: ledger.Qty, // 這裡會回 -req.Qty
               TxnAtUtc: ledger.TxnAtUtc,
               RefType: ledger.RefType,
               RefNo: ledger.RefNo
            );
        }


        //庫存結餘 / 台帳查詢
        public async Task<IReadOnlyList<InventoryBalanceResponse>> GetBalancesByProductAsync(Guid productId, CancellationToken ct = default)
        {
            //判斷該商品是否入庫
            var hasInvalidWarehouse = await _db.InventoryBalances.AnyAsync(x => x.ProductId == productId && x.WarehouseId == Guid.Empty, ct);
            if (hasInvalidWarehouse)
                throw new Exception("該商品還未入庫");
            //查詢
            var result = await _db.InventoryBalances
                .Where(x => x.ProductId == productId)
                .OrderBy(x => x.WarehouseId)
                .Select(x => new InventoryBalanceResponse(x.ProductId, x.WarehouseId, x.OnHandQty))
                .ToListAsync(ct);

            return result;
        }


        //
        public async Task<PagedResult<StockLedgerItemResponse>> GetLedgerAsync(Guid? productId,
                                                                               Guid? warehouseId,
                                                                               string? refNo,
                                                                               DateTime? fromUtc,
                                                                               DateTime? toUtc,
                                                                               int page,
                                                                               int pageSize,
                                                                               CancellationToken ct = default)
        {
            page=page<=0?1:page;
            pageSize = pageSize is < 1 or > 200 ? 20 : pageSize;
            var q = _db.StockLedgers.AsNoTracking().AsQueryable();
            // 條件：商品
            if (productId.HasValue) q = q.Where(x => x.ProductId == productId.Value);

            // 條件：倉庫
            if (warehouseId.HasValue) q = q.Where(x => x.WarehouseId == warehouseId.Value);

            // 條件：單號（模糊查）
            if (!string.IsNullOrWhiteSpace(refNo)) //.IsNullOrWhiteSpace()只有當使用者真的有輸入搜尋關鍵字時才搜尋
            {
                var keyword = refNo.Trim();//去頭尾
                q = q.Where(x => x.RefNo.Contains(keyword));//模糊查詢
            }

            // 條件：時間區間
            if (fromUtc.HasValue) q = q.Where(x => x.TxnAtUtc >= fromUtc.Value);
            if (toUtc.HasValue) q = q.Where(x => x.TxnAtUtc <= toUtc.Value);

            // 先算總筆數（給分頁用）
            var total = await q.CountAsync(ct);

            // 再拿當頁資料（最新在前）
            var items = await q
                .OrderByDescending(x => x.TxnAtUtc)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new StockLedgerItemResponse(
                    x.Id,
                    x.ProductId,
                    x.WarehouseId,
                    (int)x.TxnType,
                    x.Qty,
                    x.TxnAtUtc,
                    x.RefType,
                    x.RefNo,
                    x.Remark
                ))
                .ToListAsync(ct);

            return new PagedResult<StockLedgerItemResponse>(page, pageSize, total, items);
        }
    }
}
