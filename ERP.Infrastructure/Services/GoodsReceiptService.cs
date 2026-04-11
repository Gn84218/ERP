using ERP.Application.DTOs;
using ERP.Application.Interfaces;
using ERP.Domain.Entities;
using ERP.Domain.Enums;
using ERP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ERP.Infrastructure.Services;
//收貨單
public class GoodsReceiptService : IGoodsReceiptService
{
    private readonly AppDbContext _db;
    private readonly IInventoryService _inventory; // 用現成的入庫引擎

    public GoodsReceiptService(AppDbContext db, IInventoryService inventory)
    {
        _db = db;
        _inventory = inventory;
    }

    public async Task<GoodsReceiptResponse> CreateAsync(CreateGoodsReceiptRequest req, CancellationToken ct = default)
    {
        // 防呆：至少 1 筆明細
        if (req.Lines == null || req.Lines.Count == 0)
            throw new InvalidOperationException("收貨單必須至少包含 1 筆明細。");

        // PO 必須存在且已核准（業界常見規則）
        var po = await _db.PurchaseOrders
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == req.PurchaseOrderId, ct);

        if (po == null) throw new InvalidOperationException("找不到採購單 PurchaseOrderId。");
        if (po.Status != PurchaseOrderStatus.Approved)//Status =1為核准 2為草稿
            throw new InvalidOperationException("採購單尚未核准，不能建立收貨單。");

        // 倉庫必須存在
        var whExists = await _db.Warehouses.AnyAsync(x => x.Id == req.WarehouseId, ct);
        if (!whExists) throw new InvalidOperationException("找不到倉庫 WarehouseId。");

        // 商品存在檢查
        var productIds = req.Lines.Select(x => x.ProductId).Distinct().ToList();
        var existingCount = await _db.Products.CountAsync(x => productIds.Contains(x.Id), ct);
        if (existingCount != productIds.Count)
            throw new InvalidOperationException("收貨明細包含不存在的商品 ProductId。");

        // 產生 GRN 單號（簡化版）
        var no = "GRN" + DateTime.UtcNow.ToString("yyyyMMddHHmmss");

        var grn = new GoodsReceipt
        {
            No = no,
            PurchaseOrderId = req.PurchaseOrderId,
            WarehouseId = req.WarehouseId,
            Status = GoodsReceiptStatus.Draft,
            CreatedAtUtc = DateTime.UtcNow,
            Lines = req.Lines.Select(l => new GoodsReceiptLine
            {
                ProductId = l.ProductId,
                ReceivedQty = l.ReceivedQty,
                UnitCost = l.UnitCost
            }).ToList()
        };

        _db.GoodsReceipts.Add(grn);
        await _db.SaveChangesAsync(ct);

        return await GetByIdAsync(grn.Id, ct);
    }

    public async Task<GoodsReceiptResponse> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var grn = await _db.GoodsReceipts
            .AsNoTracking()
            .Include(x => x.Lines)
            .SingleOrDefaultAsync(x => x.Id == id, ct);

        if (grn == null) throw new InvalidOperationException("找不到收貨單。");

        return new GoodsReceiptResponse(
            grn.Id,
            grn.No,
            grn.PurchaseOrderId,
            grn.WarehouseId,
            grn.Status,
            grn.CreatedAtUtc,
            grn.PostedAtUtc,
            grn.Lines.Select(l => new GoodsReceiptLineResponse(l.Id, l.ProductId, l.ReceivedQty, l.UnitCost)).ToList()
        );
    }

    public async Task<GoodsReceiptResponse> PostAsync(Guid id, CancellationToken ct = default)
    {
        // 讀出 GRN（要更新狀態，所以不能 AsNoTracking）
        var grn = await _db.GoodsReceipts
            .Include(x => x.Lines)
            .SingleOrDefaultAsync(x => x.Id == id, ct);

        if (grn == null) throw new InvalidOperationException("找不到收貨單。");
        if (grn.Status != GoodsReceiptStatus.Draft)
            throw new InvalidOperationException("收貨單不是草稿狀態，不能過帳。");

        // ✅ Transaction：GRN 狀態改 Posted + 逐筆入庫 必須一致
        await using var tx = await _db.Database.BeginTransactionAsync(ct);

        // 1) 狀態改為 Posted（已過帳）
        grn.Status = GoodsReceiptStatus.Posted;
        grn.PostedAtUtc = DateTime.UtcNow;

        // 2) 逐筆入庫（寫台帳 + 更新結餘）
        // RefType/RefNo 讓台帳可以追溯到 GRN
        foreach (var line in grn.Lines)
        {
            if (line.ReceivedQty <= 0)
                throw new InvalidOperationException("實收數量必須大於 0。");

            await _inventory.StockInAsync(new StockInRequest(
                ProductId: line.ProductId,
                WarehouseId: grn.WarehouseId,
                Qty: line.ReceivedQty,
                RefType: "GRN",           // 台帳來源：收貨單
                RefNo: grn.No,            // 直接用 GRN 單號追溯
                Remark: "收貨過帳自動入庫"
            ), ct);
        }

        // 3) 存 GRN 狀態
        await _db.SaveChangesAsync(ct);

        // 4) Commit
        await tx.CommitAsync(ct);

        return await GetByIdAsync(grn.Id, ct);
    }
}
