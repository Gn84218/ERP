using ERP.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Domain.Entities
{
    /*
     * Shipment = 出貨單（表頭）
     * - No：出貨單號
     * - SalesOrderId：來源銷售單
     * - WarehouseId：從哪個倉庫出貨
     * - Status：Draft / Shipped
     */
    public class Shipment
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string No { get; set; } = default!;

        public Guid SalesOrderId { get; set; }
        public Guid WarehouseId { get; set; }

        public ShipmentStatus Status { get; set; } = ShipmentStatus.Draft;

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime? ShippedAtUtc { get; set; }

        public List<ShipmentLine> Lines { get; set; } = new();
    }
}
