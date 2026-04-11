using ERP.Application.DTOs;
using ERP.Application.Interfaces;
using ERP.Domain.Entities;
using ERP.Domain.Enums;
using ERP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Infrastructure.Services
{
    /*
     * ShipmentService = 出貨單服務
     * 流程：
     * SO 已核准
     * 建 Shipment
     * Ship（出貨）
     * 自動呼叫 StockOut
     */
    public class ShipmentService : IShipmentService
    {
        private readonly AppDbContext _db;
        private readonly IInventoryService _inventoryService;

        public ShipmentService(AppDbContext db, IInventoryService inventoryService)
        {
            _db = db;
            _inventoryService = inventoryService;
        }

        public async Task<ShipmentResponse> CreateAsync(CreateShipmentRequest req, CancellationToken ct = default)
        {
            // 至少要一筆明細
            if (req.Lines == null || req.Lines.Count == 0)
                throw new InvalidOperationException("出貨單必須至少包含 1 筆明細。");

            // SO 必須存在且已核准
            var so = await _db.SalesOrders
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == req.SalesOrderId, ct);

            if (so == null)
                throw new InvalidOperationException("找不到銷售單 SalesOrderId。");

            if (so.Status != SalesOrderStatus.Approved)
                throw new InvalidOperationException("銷售單尚未核准，不能建立出貨單。");

            // 倉庫必須存在
            var warehouseExists = await _db.Warehouses.AnyAsync(x => x.Id == req.WarehouseId, ct);
            if (!warehouseExists)
                throw new InvalidOperationException("找不到倉庫 WarehouseId。");

            // 商品必須存在
            var productIds = req.Lines.Select(x => x.ProductId).Distinct().ToList();
            var existingProductCount = await _db.Products.CountAsync(x => productIds.Contains(x.Id), ct);
            if (existingProductCount != productIds.Count)
                throw new InvalidOperationException("出貨明細包含不存在的商品 ProductId。");

            // 產生單號（簡化版）
            var no = "SHIP" + DateTime.UtcNow.ToString("yyyyMMddHHmmss");

            var shipment = new Shipment
            {
                No = no,
                SalesOrderId = req.SalesOrderId,
                WarehouseId = req.WarehouseId,
                Status = ShipmentStatus.Draft,
                CreatedAtUtc = DateTime.UtcNow,
                Lines = req.Lines.Select(l => new ShipmentLine
                {
                    ProductId = l.ProductId,
                    ShippedQty = l.ShippedQty
                }).ToList()
            };

            _db.Shipments.Add(shipment);
            await _db.SaveChangesAsync(ct);

            return await GetByIdAsync(shipment.Id, ct);
        }

        public async Task<ShipmentResponse> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            var shipment = await _db.Shipments
                .AsNoTracking()
                .Include(x => x.Lines)//Include 出貨單明細
                .SingleOrDefaultAsync(x => x.Id == id, ct);

            if (shipment == null)
                throw new InvalidOperationException("找不到出貨單。");

            return new ShipmentResponse(
                shipment.Id,
                shipment.No,
                shipment.SalesOrderId,
                shipment.WarehouseId,
                shipment.Status,
                shipment.CreatedAtUtc,
                shipment.ShippedAtUtc,
                shipment.Lines.Select(l => new ShipmentLineResponse(
                    l.Id,
                    l.ProductId,
                    l.ShippedQty
                )).ToList()
            );
        }

        public async Task<ShipmentResponse> ShipAsync(Guid id, CancellationToken ct = default)
        {
            var shipment = await _db.Shipments
                .Include(x => x.Lines)
                .SingleOrDefaultAsync(x => x.Id == id, ct);

            if (shipment is null)
                throw new InvalidOperationException("找不到出貨單。");

            if (shipment.Status != ShipmentStatus.Draft)
                throw new InvalidOperationException("出貨單不是 Draft 狀態，不能出貨。");

            // Transaction：改狀態 + 逐筆扣庫存 必須一致
            await using var tx = await _db.Database.BeginTransactionAsync(ct);

            shipment.Status = ShipmentStatus.Shipped;
            shipment.ShippedAtUtc = DateTime.UtcNow;

            foreach (var line in shipment.Lines)
            {
                if (line.ShippedQty <= 0)
                    throw new InvalidOperationException("出貨數量必須大於 0。");

                // 呼叫你現有的出庫引擎
                await _inventoryService.StockOutAsync(new StockOutRequest(
                    ProductId: line.ProductId,
                    WarehouseId: shipment.WarehouseId,
                    Qty: line.ShippedQty,
                    RefType: "SHIP",
                    RefNo: shipment.No,
                    Remark: "出貨過帳自動扣庫存"
                ), ct);
            }

            await _db.SaveChangesAsync(ct);
            await tx.CommitAsync(ct);

            return await GetByIdAsync(shipment.Id, ct);
        }
    }
}
