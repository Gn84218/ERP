using ERP.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//客戶服務介面
namespace ERP.Application.Interfaces
{
    public interface ICustomerService
    {
        Task<CustomerResponse> CreateAsync(CreateCustomerRequest req, CancellationToken ct = default);

        Task<IReadOnlyList<CustomerResponse>> GetAllAsync(CancellationToken ct = default);
    }
}
