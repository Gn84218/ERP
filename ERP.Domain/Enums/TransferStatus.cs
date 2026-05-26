using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Domain.Enums
{
    /*
    * 調撥狀態
    * 未來可新增 是否允調撥等欄位
    */
    public enum TransferStatus
    {
        Draft = 1,//前置
        Posted = 2 //完成
    }
}
