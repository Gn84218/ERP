using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Domain.Entities
{
    public class Warehouse
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        // 商品代碼（內部使用）
        // 例如：P001、ERP-1001
        public string Code { get; set; } = default!;
        public string Name { get; set; } = default!;
        // 是否啟用
        public bool IsActive { get; set; } = true;
    }
}
