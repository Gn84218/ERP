using ERP.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Domain.Entities
{
 /*
 * PurchaseOrder = 採購單（表頭）
 * - No：採購單號（例如 PO0001）
 * - SupplierId：供應商
 * - Status：狀態 Draft/Approved
 */
    public class PurchaseOrder
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string No { get; set; } = default!; // 採購單號（唯一）
        public Guid SupplierId { get; set; }

        public PurchaseOrderStatus Status { get; set; } = PurchaseOrderStatus.Draft;

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime? ApprovedAtUtc { get; set; }

        // 明細
        public List<PurchaseOrderLine> Lines { get; set; } = new();
    }
}
