using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Domain.Entities
{
    /*
     * 調撥明細
     */
    public class TransferLine
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid TransferId { get; set; }//調撥單 表單 關聯健

        public Guid ProductId { get; set; }

        public decimal Qty { get; set; }
    }
}
