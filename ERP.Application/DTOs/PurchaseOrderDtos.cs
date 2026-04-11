using ERP.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Application.DTOs
{
    /*
  * 建立採購單明細（表身）
  */
    public record CreatePurchaseOrderLineRequest(
        Guid ProductId,
        decimal Qty,
        decimal UnitCost
    );

    /*
     * 建立採購單（表頭 + 表身）
     */
    public record CreatePurchaseOrderRequest(
        Guid SupplierId,
        List<CreatePurchaseOrderLineRequest> Lines
    );

    /*
     * PO 回傳明細
     */
    public record PurchaseOrderLineResponse(
        Guid Id,
        Guid ProductId,
        decimal Qty,
        decimal UnitCost
    );

    /*
     * PO 回傳表頭 + 明細
     */
    public record PurchaseOrderResponse(
        Guid Id,
        string No,
        Guid SupplierId,
        PurchaseOrderStatus Status,
        DateTime CreatedAtUtc,
        DateTime? ApprovedAtUtc,
        IReadOnlyList<PurchaseOrderLineResponse> Lines
    );
}
