using ERP.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//銷售單相關的 DTOs (SO)
namespace ERP.Application.DTOs
{
    //
    public record CreateSalesOrderLineRequest(
     Guid ProductId,
     decimal Qty,
     decimal UnitPrice
    );
    //
    public record CreateSalesOrderRequest(
        Guid CustomerId,
        List<CreateSalesOrderLineRequest> Lines
    );

    public record SalesOrderLineResponse(
        Guid Id,
        Guid ProductId,
        decimal Qty,
        decimal UnitPrice
    );

    public record SalesOrderResponse(
        Guid Id,
        string No,
        Guid CustomerId,
        SalesOrderStatus Status,
        DateTime CreatedAtUtc,
        DateTime? ApprovedAtUtc,
        IReadOnlyList<SalesOrderLineResponse> Lines
    );
}
