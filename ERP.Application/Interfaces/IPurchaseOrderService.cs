using ERP.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Application.Interfaces
{
    public interface IPurchaseOrderService
    {
        Task<PurchaseOrderResponse> CreateAsync(CreatePurchaseOrderRequest req, CancellationToken ct = default);

        Task<PurchaseOrderResponse> GetByIdAsync(Guid id, CancellationToken ct = default);

        // 核准 PO：Draft → Approved
        Task<PurchaseOrderResponse> ApproveAsync(Guid id, CancellationToken ct = default);
    }
}
