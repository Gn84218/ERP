using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Domain.Entities
{
 /*
 * 銷售單明細
 */
    public class SalesOrderLine
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid SalesOrderId { get; set; }

        public Guid ProductId { get; set; }

        public decimal Qty { get; set; }

        public decimal UnitPrice { get; set; }
    }
}
