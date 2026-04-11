using ERP.Application.DTOs;
using ERP.Application.Interfaces;
using ERP.Domain.Entities;
using ERP.Domain.Enums;
using ERP.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ERP.Infrastructure.Services
{
    public class PurchaseOrderService : IPurchaseOrderService
    {
        private readonly AppDbContext _db;
        public PurchaseOrderService(AppDbContext db) => _db = db;

        public async Task<PurchaseOrderResponse> CreateAsync(CreatePurchaseOrderRequest req, CancellationToken ct = default)
        {
            // 基本防呆：至少要有一筆明細
            if (req.Lines is null || req.Lines.Count == 0)
                throw new InvalidOperationException("採購單必須至少包含 1 筆明細。");

            // 確認供應商存在
            var supplierExists = await _db.Suppliers.AnyAsync(x => x.Id == req.SupplierId, ct);
            if (!supplierExists) throw new InvalidOperationException("找不到供應商 SupplierId。");

            // 確認商品存在（避免 PO 放不存在的商品）
            var productIds = req.Lines.Select(x => x.ProductId).Distinct().ToList();
            var existingProductCount = await _db.Products.CountAsync(x => productIds.Contains(x.Id), ct);
            if (existingProductCount != productIds.Count)
                throw new InvalidOperationException("採購明細包含不存在的商品 ProductId。");

            // 產生 PO 單號（先用簡化版：PO + yyyyMMddHHmmss）
            // 之後可改成更漂亮：PO000001 + 專用序號表
            var no = "PO" + DateTime.UtcNow.ToString("yyyyMMddHHmmss");

            var po = new PurchaseOrder
            {
                No = no,
                SupplierId = req.SupplierId,
                Status = PurchaseOrderStatus.Draft,
                CreatedAtUtc = DateTime.UtcNow,
                Lines = req.Lines.Select(l => new PurchaseOrderLine
                {
                    ProductId = l.ProductId,
                    Qty = l.Qty,
                    UnitCost = l.UnitCost
                }).ToList()
            };

            _db.PurchaseOrders.Add(po);
            await _db.SaveChangesAsync(ct);

            return await GetByIdAsync(po.Id, ct);
        }

        public async Task<PurchaseOrderResponse> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            var po = await _db.PurchaseOrders
                .AsNoTracking()
                .Include(x => x.Lines)
                .SingleOrDefaultAsync(x => x.Id == id, ct);

            if (po is null) throw new InvalidOperationException("找不到採購單。");

            return new PurchaseOrderResponse(
                po.Id,
                po.No,
                po.SupplierId,
                po.Status,
                po.CreatedAtUtc,
                po.ApprovedAtUtc,
                po.Lines.Select(l => new PurchaseOrderLineResponse(l.Id, l.ProductId, l.Qty, l.UnitCost)).ToList()
            );
        }

        public async Task<PurchaseOrderResponse> ApproveAsync(Guid id, CancellationToken ct = default)
        {
            // 讀出 PO（要更新所以不能 AsNoTracking）
            var po = await _db.PurchaseOrders
                .Include(x => x.Lines)
                .SingleOrDefaultAsync(x => x.Id == id, ct);

            if (po is null) throw new InvalidOperationException("找不到採購單。");

            // 只有 Draft 才能核准
            if (po.Status != PurchaseOrderStatus.Draft)
                throw new InvalidOperationException("採購單不是草稿狀態，不能核准。");

            po.Status = PurchaseOrderStatus.Approved;
            po.ApprovedAtUtc = DateTime.UtcNow;

            await _db.SaveChangesAsync(ct);

            return await GetByIdAsync(po.Id, ct);
        }
    }
}
