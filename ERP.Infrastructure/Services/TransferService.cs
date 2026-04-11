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

// 調波
namespace ERP.Infrastructure.Services
{
    public class TransferService : ITransferService
    {
        private readonly AppDbContext _db;
        private readonly IInventoryService _inventory;//現成庫存服務

        public TransferService(AppDbContext db, IInventoryService inventory)
        {
            _db = db;
            _inventory = inventory;
        }

        public async Task<TransferResponse> CreateAsync(CreateTransferRequest req, CancellationToken ct)
        {
            if (req.Lines == null || req.Lines.Count == 0)
                throw new InvalidOperationException("調撥單至少一筆明細");

            if (req.FromWarehouseId == req.ToWarehouseId)
                throw new InvalidOperationException("來源與目標倉庫不能相同");

            var no = "TR" + DateTime.UtcNow.ToString("yyyyMMddHHmmss");

            var entity = new Transfer
            {
                No = no,
                FromWarehouseId = req.FromWarehouseId,
                ToWarehouseId = req.ToWarehouseId,
                Status = TransferStatus.Draft,
                Lines = req.Lines.Select(l => new TransferLine
                {
                    ProductId = l.ProductId,
                    Qty = l.Qty
                }).ToList()
            };

            _db.Transfers.Add(entity);
            await _db.SaveChangesAsync(ct);

            return await GetByIdAsync(entity.Id, ct);
        }

        public async Task<TransferResponse> GetByIdAsync(Guid id, CancellationToken ct)
        {
            var t = await _db.Transfers
                .Include(x => x.Lines)
                .SingleOrDefaultAsync(x => x.Id == id, ct);

            if (t == null) throw new InvalidOperationException("找不到調撥單");

            return new TransferResponse(
                t.Id,
                t.No,
                t.FromWarehouseId,
                t.ToWarehouseId,
                t.Status,
                t.CreatedAtUtc,
                t.PostedAtUtc,
                t.Lines.Select(l => new TransferLineResponse(l.Id, l.ProductId, l.Qty)).ToList()
            );
        }

        public async Task<TransferResponse> PostAsync(Guid id, CancellationToken ct)
        {
            var t = await _db.Transfers
                .Include(x => x.Lines)
                .SingleOrDefaultAsync(x => x.Id == id, ct);

            if (t == null) throw new InvalidOperationException("找不到調撥單");

            if (t.Status != TransferStatus.Draft)
                throw new InvalidOperationException("不是 Draft 不能過帳");

            //交易確保資料一致性
            await using var tx = await _db.Database.BeginTransactionAsync(ct);

            t.Status = TransferStatus.Posted;
            t.PostedAtUtc = DateTime.UtcNow;

            foreach (var line in t.Lines)
            {
                // 來源倉庫 → 出庫
                await _inventory.StockOutAsync(new StockOutRequest(
                    line.ProductId,
                    t.FromWarehouseId,
                    line.Qty,
                    "TRANSFER",
                    t.No,
                    "調撥出庫"
                ), ct);

                // 目標倉庫 → 入庫
                await _inventory.StockInAsync(new StockInRequest(
                    line.ProductId,
                    t.ToWarehouseId,
                    line.Qty,
                    "TRANSFER",
                    t.No,
                    "調撥入庫"
                ), ct);
            }

            await _db.SaveChangesAsync(ct);
            await tx.CommitAsync(ct); //確保出庫和入庫要一起成功或一起失敗

            return await GetByIdAsync(t.Id, ct);
        }
    }
}
