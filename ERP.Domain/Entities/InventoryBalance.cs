using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Domain.Entities
{
    // 庫存餘額表（記錄目前實際剩餘庫存數量）
    public class InventoryBalance
    {
        // 主鍵ID（每一筆庫存餘額資料的唯一識別碼）
        public Guid Id { get; set; } = Guid.NewGuid();

        // 商品ID（對應 Product 表）
        // 表示是哪一個商品的庫存
        public Guid ProductId { get; set; }

        // 倉庫ID（對應 Warehouse 表）
        // 表示是哪一個倉庫的庫存
        public Guid WarehouseId { get; set; }

        // 現有庫存數量（目前結餘）
        // 例如：目前倉庫內還剩 150 件
        public decimal OnHandQty { get; set; }

        // 最後更新時間（UTC）
        // 每次庫存變動後都會更新這個時間
        public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
    }

}
