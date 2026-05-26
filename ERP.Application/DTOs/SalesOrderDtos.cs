using ERP.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//銷售單相關的 DTOs (SO)
namespace ERP.Application.DTOs
{
    //請求明細
    public record CreateSalesOrderLineRequest(
     Guid ProductId,
     decimal Qty,
     decimal UnitPrice
    );
    //請求 標頭+明細
    public record CreateSalesOrderRequest(
        Guid CustomerId,
        List<CreateSalesOrderLineRequest> Lines
    );
    //返回明細
    public record SalesOrderLineResponse(
        Guid Id,
        Guid ProductId,
        decimal Qty,//數量
        decimal UnitPrice,//單價
        string? ProductName = null
    );

    //返回 標頭+明細
    public record SalesOrderResponse(
        Guid Id,
        string No,//單號
        Guid CustomerId,//客戶ID
        SalesOrderStatus Status,//狀態
        DateTime CreatedAtUtc,//建立時間
        DateTime? ApprovedAtUtc,//核准時間
        IReadOnlyList<SalesOrderLineResponse> Lines
    );
}
