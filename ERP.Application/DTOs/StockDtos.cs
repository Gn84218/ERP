using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Application.DTOs
{
    // 庫存相關的
   
        // 入庫 Request：哪個商品、哪個倉庫、入多少、單據來源
        public record StockInRequest(
           Guid ProductId,       // 商品Id（Product）
           Guid WarehouseId,     // 倉庫Id（Warehouse）
           decimal Qty,          // 入庫數量（必須 > 0）
           string RefType,       // 單據類型，例如 "PO"（採購單）
           string RefNo,         // 單號，例如 "PO0001"
           string? Remark        // 備註：可填入「第一筆採購入庫」之類
        );

        // 回傳：入庫寫進台帳的結果
        public record StockInResponse(
           Guid LedgerId,        // 台帳Id（StockLedger.Id）
           Guid ProductId,
           Guid WarehouseId,
           decimal Qty,
           DateTime TxnAtUtc,    // 異動時間（UTC）
           string RefType,
           string RefNo
        );


    //StockOutRequest = 出庫請求（API Body）
    // Qty 一樣用「正數」傳入（例如 5）
    // 內部寫台帳時才會轉成負數（-5）
    public record StockOutRequest(
        Guid ProductId,       // 商品Id
        Guid WarehouseId,     // 倉庫Id
        decimal Qty,          // 出庫數量（必須 > 0）
        string RefType,       // 單據類型，例如 "SO"
        string RefNo,         // 單號，例如 "SO0001"
        string? Remark        // 備註
    );

    /*
     * StockOutResponse = 出庫結果
     * Qty 會回傳「台帳寫入的 Qty」，也就是負數（例如 -5）
     */
    public record StockOutResponse(
        Guid LedgerId,
        Guid ProductId,
        Guid WarehouseId,
        decimal Qty,
        DateTime TxnAtUtc,
        string RefType,
        string RefNo
    );

    //庫存結餘 / 台帳查詢
    public record InventoryBalanceResponse(
    Guid ProductId,
    Guid WarehouseId,
    decimal OnHandQty
);
}
