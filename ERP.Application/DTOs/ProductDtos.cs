using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Application.DTOs
{
    // 這裡定義了與商品相關的 DTO（Data Transfer Object）類別
    public class ProductDtos
    {
        // 建立商品用的 Request
        public record CreateProductRequest(string Sku, string Name, decimal Cost, decimal Price);
        // 更新商品用的 Request
        public record ProductResponse(Guid Id, string Sku, string Name, decimal Cost, decimal Price, bool IsActive);
    }
}
