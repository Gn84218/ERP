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
 * 銷售單服務
 * 流程：
 * Draft -> Approved
 */
    public class SalesOrderService : ISalesOrderService
    {
        private readonly AppDbContext _db;

        public SalesOrderService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<SalesOrderResponse> CreateAsync(CreateSalesOrderRequest req, CancellationToken ct = default)
        {
            // 至少要一筆明細
            if (req.Lines is null || req.Lines.Count == 0)
                throw new InvalidOperationException("銷售單必須至少包含 1 筆明細。");

            // 客戶必須存在
            var customerExists = await _db.Customers.AnyAsync(x => x.Id == req.CustomerId, ct);
            if (!customerExists)
                throw new InvalidOperationException("找不到客戶 CustomerId。");

            // 商品必須存在
            var productIds = req.Lines.Select(x => x.ProductId).Distinct().ToList();
            var existingProductCount = await _db.Products.CountAsync(x => productIds.Contains(x.Id), ct);
            if (existingProductCount != productIds.Count)
                throw new InvalidOperationException("銷售單明細包含不存在的商品 ProductId。");

            // 產生單號（簡化版）
            var no = "SO" + DateTime.UtcNow.ToString("yyyyMMddHHmmss");

            var so = new SalesOrder
            {
                No = no,
                CustomerId = req.CustomerId,
                Status = SalesOrderStatus.Draft,
                CreatedAtUtc = DateTime.UtcNow,
                //明細
                Lines = req.Lines.Select(l => new SalesOrderLine
                {
                    ProductId = l.ProductId,
                    Qty = l.Qty,
                    UnitPrice = l.UnitPrice
                }).ToList()
            };

            _db.SalesOrders.Add(so);
            await _db.SaveChangesAsync(ct);

            return await GetByIdAsync(so.Id, ct);
        }

        public async Task<SalesOrderResponse> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            var so = await _db.SalesOrders
                .AsNoTracking()
                .Include(x => x.Lines)
                .SingleOrDefaultAsync(x => x.Id == id, ct);//取1比 (唯一)

            if (so is null)
                throw new InvalidOperationException("找不到銷售單。");

            return new SalesOrderResponse(
                so.Id,
                so.No,
                so.CustomerId,
                so.Status,
                so.CreatedAtUtc,
                so.ApprovedAtUtc,
                so.Lines.Select(l => new SalesOrderLineResponse(
                    l.Id,
                    l.ProductId,
                    l.Qty,
                    l.UnitPrice
                )).ToList()
            );
        }

        public async Task<SalesOrderResponse> ApproveAsync(Guid id, CancellationToken ct = default)
        {
            var so = await _db.SalesOrders
                .Include(x => x.Lines)
                .SingleOrDefaultAsync(x => x.Id == id, ct);//取1比 (唯一)

            if (so is null)
                throw new InvalidOperationException("找不到銷售單。");

            if (so.Status != SalesOrderStatus.Draft)
                throw new InvalidOperationException("銷售單不是 Draft 狀態，不能核准。");

            so.Status = SalesOrderStatus.Approved;
            so.ApprovedAtUtc = DateTime.UtcNow;

            await _db.SaveChangesAsync(ct);

            return await GetByIdAsync(so.Id, ct);
        }
    }
}
