using ERP.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Application.DTOs
{
    /*
     * 建立 GRN 明細
     * 這裡先用「商品 + 實收數量 + 單價」
     * （第一版不強制綁 PO line，先快速跑通流程）
     */
    public record CreateGoodsReceiptLineRequest(
        Guid ProductId,
        decimal ReceivedQty,
        decimal UnitCost
    );

    /*
     * 建立 GRN（表頭 + 明細）
     * - PurchaseOrderId：來源 PO
     * - WarehouseId：入庫倉庫
     */
    public record CreateGoodsReceiptRequest(
        Guid PurchaseOrderId,
        Guid WarehouseId,
        List<CreateGoodsReceiptLineRequest> Lines
    );
    //回傳明細
    public record GoodsReceiptLineResponse(
        Guid Id,
        Guid ProductId,
        decimal ReceivedQty,
        decimal UnitCost
    );
    //回傳表頭 + 明細
    public record GoodsReceiptResponse(
        Guid Id,
        string No,
        Guid PurchaseOrderId,
        Guid WarehouseId,
        GoodsReceiptStatus Status,
        DateTime CreatedAtUtc,
        DateTime? PostedAtUtc,
        IReadOnlyList<GoodsReceiptLineResponse> Lines
    );
}
