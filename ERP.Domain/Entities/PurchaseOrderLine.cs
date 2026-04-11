using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Domain.Entities
{
 /*
 * PurchaseOrderLine = 採購單明細（表身）
 * - ProductId：採購的商品
 * - Qty：數量
 * - UnitCost：單價（成本）
 */
    public class PurchaseOrderLine
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid PurchaseOrderId { get; set; }
        public Guid ProductId { get; set; }

        public decimal Qty { get; set; }
        public decimal UnitCost { get; set; }
    }
}
