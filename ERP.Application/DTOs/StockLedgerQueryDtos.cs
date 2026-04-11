using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Application.DTOs
{
    /*
  * StockLedgerItemResponse = 台帳列表項目(每筆詳細)
  * 中文：每一筆庫存異動（入庫/出庫/調撥/調整）
  */
    public record StockLedgerItemResponse(
        Guid Id,               // 台帳Id
        Guid ProductId,        // 商品Id
        Guid WarehouseId,      // 倉庫Id
        int TxnType,           // 異動類型（enum 以 int 回傳，前端好處理）
        decimal Qty,           // 數量（入庫+，出庫-）
        DateTime TxnAtUtc,     // 異動時間
        string RefType,        // 單據類型（PO/SO/GRN...）
        string RefNo,          // 單號
        string? Remark         // 備註
    );

    /*
     * PagedResult<T> = 分頁回應格式
     * 中文：帶總筆數的分頁（面試很加分）
     */
    public record PagedResult<T>(
        int Page,
        int PageSize,
        int TotalCount,
        IReadOnlyList<T> Items
    );
}
