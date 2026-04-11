using ERP.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Domain.Entities
{
    /*
     * Transfer = 調撥單
     * - FromWarehouseId：來源倉庫
     * - ToWarehouseId：目標倉庫
     * 丹頭
     */
    public class Transfer
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string No { get; set; } = default!;

        public Guid FromWarehouseId { get; set; } //出貨倉庫
        public Guid ToWarehouseId { get; set; }   //收貨倉庫

        public TransferStatus Status { get; set; } = TransferStatus.Draft;

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime? PostedAtUtc { get; set; }

        public List<TransferLine> Lines { get; set; } = new();
    }
}
