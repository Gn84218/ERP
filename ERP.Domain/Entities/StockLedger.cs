using ERP.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Domain.Entities
{
    // 庫存流水帳（記錄每一次庫存異動的歷史）
    public class StockLedger
    {
        // 流水帳唯一識別碼（主鍵）
        public Guid Id { get; set; } = Guid.NewGuid();

        // 商品ID（關聯 Product 表）
        public Guid ProductId { get; set; }

        // 倉庫ID（關聯 Warehouse 表）
        public Guid WarehouseId { get; set; }

        // 異動類型（Enum）
        // 例如：入庫、出庫、調撥、盤點
        public StockTxnType TxnType { get; set; }

        // 異動數量
        // 入庫為正數，例如 +10
        // 出庫為負數，例如 -5
        public decimal Qty { get; set; }

        // 異動時間（UTC）
        // 記錄這筆庫存變動發生的時間
        public DateTime TxnAtUtc { get; set; } = DateTime.UtcNow;

        // 來源單據類型
        // 例如：
        // "PO" = 採購單 (Purchase Order)
        // "SO" = 銷售單 (Sales Order)
        // "ADJ" = 庫存調整
        public string RefType { get; set; } = default!;

        // 來源單據編號
        // 例如：
        // "PO0001"
        // "SO20240101"
        public string RefNo { get; set; } = default!;

        // 備註說明（可空白）
        // 例如：盤點修正、破損、人工調整原因
        public string? Remark { get; set; }
    }

}
