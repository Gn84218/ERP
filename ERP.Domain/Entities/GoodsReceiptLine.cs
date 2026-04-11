using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Domain.Entities
{
    /*
  * GoodsReceiptLine = 收貨單明細
  * - ProductId：收了哪個商品
  * - ReceivedQty：實收數量
  * - UnitCost：成本（通常跟 PO 一樣，也可允許調整）
  */
    public class GoodsReceiptLine
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid GoodsReceiptId { get; set; }
        public Guid ProductId { get; set; }

        public decimal ReceivedQty { get; set; }
        public decimal UnitCost { get; set; }
    }
}
