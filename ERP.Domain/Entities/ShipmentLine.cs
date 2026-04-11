using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Domain.Entities
{
 /*
 * ShipmentLine = 出貨單明細
 * - ProductId：出哪個商品
 * - ShippedQty：實際出貨數量
 */
    public class ShipmentLine
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid ShipmentId { get; set; }

        public Guid ProductId { get; set; }

        public decimal ShippedQty { get; set; }
    }
}
