using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Domain.Enums
{
    public enum StockTxnType
    {
        PurchaseReceipt = 1, // 採購入庫
        SalesShipment = 2,   // 銷售出庫
        TransferOut = 3,     // 調撥出
        TransferIn = 4,      // 調撥入
        Adjustment = 5       // 調整/盤點差異
    }
}
