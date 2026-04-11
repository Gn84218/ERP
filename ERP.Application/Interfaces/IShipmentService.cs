using ERP.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Application.Interfaces
{
    /*
     * 出貨單服務
     * 1. 建立出貨單
     * 2. 查單筆出貨單
     * 3. 出貨（Draft -> Shipped，並自動扣庫存）
     */
    public interface IShipmentService
    {
        Task<ShipmentResponse> CreateAsync(CreateShipmentRequest req, CancellationToken ct = default);

        Task<ShipmentResponse> GetByIdAsync(Guid id, CancellationToken ct = default);

        Task<ShipmentResponse> ShipAsync(Guid id, CancellationToken ct = default);
    }
}
