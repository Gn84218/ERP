using ERP.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Domain.Entities
{
    /*
  * GoodsReceipt = 收貨單（GRN 表頭）
  * - No：收貨單號（GRN0001 之類）
  * - PurchaseOrderId：來源 PO
  * - WarehouseId：入哪個倉庫（進貨通常指定一個倉庫）
  * - Status：Draft / Posted
  */
    public class GoodsReceipt
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string No { get; set; } = default!;
        public Guid PurchaseOrderId { get; set; }
        public Guid WarehouseId { get; set; }

        public GoodsReceiptStatus Status { get; set; } = GoodsReceiptStatus.Draft;

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime? PostedAtUtc { get; set; }

        public List<GoodsReceiptLine> Lines { get; set; } = new();
    }
}
