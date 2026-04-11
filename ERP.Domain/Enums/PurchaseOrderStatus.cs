using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Domain.Enums
{
 /*
 * 採購單狀態：
 * 1 = Draft = 草稿（可修改）
 * 2 = Approved = 已核准（通常就不能亂改，下一步會做收貨 GRN）
 */
    public enum PurchaseOrderStatus
    {
        Draft = 1,
        Approved = 2
    }
}
