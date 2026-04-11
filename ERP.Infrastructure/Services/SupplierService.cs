using ERP.Application.DTOs;
using ERP.Application.Interfaces;
using ERP.Domain.Entities;
using ERP.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ERP.Infrastructure.Services
{
    //供應商
    public class SupplierService : ISupplierService
    {
        private readonly AppDbContext _db;
        public SupplierService(AppDbContext db)
        {
            _db = db;
        }
        //創建供應商
        public async Task<SupplierResponse> CreateAsync(CreateSupplierRequest req, CancellationToken ct = default)
        {
            var code = req.Code.Trim().ToUpperInvariant();

            var exists = await _db.Suppliers.AnyAsync(x => x.Code == code, ct);
            if (exists) throw new InvalidOperationException($"供應商代碼已存在：{code}");

            var entity = new Supplier
            {
                Code = code,
                Name = req.Name.Trim(),
                IsActive = true
            };

            _db.Suppliers.Add(entity);
            await _db.SaveChangesAsync(ct);

            return new SupplierResponse(entity.Id, entity.Code, entity.Name, entity.IsActive);
        }

        //獲取所有供應商
        public async Task<IReadOnlyList<SupplierResponse>> GetAllAsync(CancellationToken ct = default)
        {
            return await _db.Suppliers
                .OrderBy(x => x.Code)
                .Select(x => new SupplierResponse(x.Id, x.Code, x.Name, x.IsActive))
                .ToListAsync(ct);
        }
    }
}
