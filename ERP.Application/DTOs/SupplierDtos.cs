using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Application.DTOs
{
    /* 建立供應商 Request */
    public record CreateSupplierRequest(string Code, string Name);

    /* 供應商 Response */
    public record SupplierResponse(Guid Id, string Code, string Name, bool IsActive);
}
