using ERP.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Application.Interfaces
{
    public interface ITransferService
    {
        Task<TransferResponse> CreateAsync(CreateTransferRequest req, CancellationToken ct = default);

        Task<TransferResponse> GetByIdAsync(Guid id, CancellationToken ct = default);

        Task<TransferResponse> PostAsync(Guid id, CancellationToken ct = default);
    }
}
