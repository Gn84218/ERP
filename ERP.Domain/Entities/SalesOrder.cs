using ERP.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Domain.Entities
{
  /*
  * SalesOrder = 銷售單（表頭）
  */
    public class SalesOrder
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string No { get; set; } = default!; // SO0001

        public Guid CustomerId { get; set; } // 客戶

        public SalesOrderStatus Status { get; set; } = SalesOrderStatus.Draft;

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime? ApprovedAtUtc { get; set; }

        public List<SalesOrderLine> Lines { get; set; } = new();
    }
}
