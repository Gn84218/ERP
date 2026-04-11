using ERP.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Application.DTOs
{
    /*
     * 建立出貨單明細
     */
    public record CreateShipmentLineRequest(
        Guid ProductId,
        decimal ShippedQty
    );

    /*
     * 建立出貨單
     * - SalesOrderId：來源 SO
     * - WarehouseId：從哪個倉庫出貨
     */
    public record CreateShipmentRequest(
        Guid SalesOrderId,
        Guid WarehouseId,
        List<CreateShipmentLineRequest> Lines
    );

    public record ShipmentLineResponse(
        Guid Id,
        Guid ProductId,
        decimal ShippedQty
    );

    public record ShipmentResponse(
        Guid Id,
        string No,
        Guid SalesOrderId,
        Guid WarehouseId,
        ShipmentStatus Status,
        DateTime CreatedAtUtc,
        DateTime? ShippedAtUtc,
        IReadOnlyList<ShipmentLineResponse> Lines
    );
}
