using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Domain.Entities
{
    /*
  * Supplier = 供應商主檔
  */
    public class Supplier
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Code { get; set; } = default!; // 供應商代碼（唯一）
        public string Name { get; set; } = default!; // 供應商名稱

        public bool IsActive { get; set; } = true;
    }
}
