using ERP.Application.DTOs;
using ERP.Application.Interfaces;
using ERP.Domain.Entities;
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
  * 客戶主檔服務
  */
    public class CustomerService : ICustomerService
    {
        private readonly AppDbContext _db;

        public CustomerService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<CustomerResponse> CreateAsync(CreateCustomerRequest req, CancellationToken ct = default)
        {
            var code = req.Code.Trim().ToUpperInvariant();

            var exists = await _db.Customers.AnyAsync(x => x.Code == code, ct);
            if (exists)
                throw new InvalidOperationException($"客戶代碼已存在：{code}");

            var entity = new Customer
            {
                Code = code,
                Name = req.Name.Trim(),
                IsActive = true
            };

            _db.Customers.Add(entity);
            await _db.SaveChangesAsync(ct);

            return new CustomerResponse(entity.Id, entity.Code, entity.Name, entity.IsActive);
        }

        public async Task<IReadOnlyList<CustomerResponse>> GetAllAsync(CancellationToken ct = default)
        {
            return await _db.Customers
                .OrderBy(x => x.Code)
                .Select(x => new CustomerResponse(x.Id, x.Code, x.Name, x.IsActive))
                .ToListAsync(ct);
        }
    }
}
