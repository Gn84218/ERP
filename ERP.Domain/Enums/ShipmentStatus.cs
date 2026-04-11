using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Domain.Enums
{
 /*
 * 出貨單狀態
 * Draft = 草稿
 * Shipped = 已出貨（已扣庫存）
 */
    public enum ShipmentStatus
    {
        Draft = 1,
        Shipped = 2
    }
}
