using ERP.Domain.Entities;
using ERP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ERP.Application.DTOs.WarehouseDtos;

namespace ERP.Application.Interfaces.Services
{
    // 倉庫 ：建立、查全部（先不用分頁，倉庫通常不會很多）
    public class WarehouseService : IWarehouseService
    {
        private readonly AppDbContext _db;
        //建構子簡寫
        public WarehouseService(AppDbContext db) => _db = db;
       

        public async Task<WarehouseResponse> CreateAsync(CreateWarehouseRequest req, CancellationToken ct = default)
        {
            // 倉庫代碼通常會統一大寫
            var code = req.Code.Trim().ToUpperInvariant();

            // 倉庫代碼唯一
            var exists = await _db.Warehouses.AnyAsync(x => x.Code == code, ct);
            if (exists) throw new InvalidOperationException($"倉庫代碼已存在：{code}");

            var entity = new Warehouse
            {
                Code = code,
                Name = req.Name.Trim(),
                IsActive = true
            };

            _db.Warehouses.Add(entity);
            await _db.SaveChangesAsync(ct);

            return new WarehouseResponse(entity.Id, entity.Code, entity.Name, entity.IsActive);
        }

        public async Task<IReadOnlyList<WarehouseResponse>> GetAllAsync(CancellationToken ct = default)
        {
            // 倉庫通常不多，先做 GetAll
            return await _db.Warehouses
                .OrderBy(x => x.Code)
                .Select(x => new WarehouseResponse(x.Id, x.Code, x.Name, x.IsActive))
                .ToListAsync(ct);
        }
    }
}
