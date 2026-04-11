using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Domain.Enums
{
    /*
 * 收貨單狀態：
 * Draft = 草稿（可修改）
 * Posted = 已過帳（已入庫，不能亂改）
 */
    public enum GoodsReceiptStatus
    {
        Draft = 1,
        Posted = 2
    }
}
