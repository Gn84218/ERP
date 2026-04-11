using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Domain.Entities
{
    //default! 保證這個字串之後一定會被賦值，不會是 null
    public class Product
    {
        // 商品唯一識別碼（主鍵）
        public Guid Id { get; set; } = Guid.NewGuid();

        // 商品編號（SKU = Stock Keeping Unit）
        // 例如：A001、IPHONE15-128
        public string Sku { get; set; } = default!;

        // 商品名稱
        public string Name { get; set; } = default!;

        // 成本價（公司進貨成本）
        public decimal Cost { get; set; }

        // 售價（賣給客人的價格）
        public decimal Price { get; set; }

        // 是否啟用（軟刪除概念）
        // true = 上架中
        // false = 停售
        public bool IsActive { get; set; } = true;
    }
}
