using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Application.DTOs
{
    // 與倉庫相關
    public class WarehouseDtos
    {
        // 建立倉庫用的 Request
        public record CreateWarehouseRequest(string Code, string Name);

        // 倉庫 Response
        public record WarehouseResponse(Guid Id, string Code, string Name, bool IsActive);
    }
}
