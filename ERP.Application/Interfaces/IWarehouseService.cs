using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ERP.Application.DTOs.WarehouseDtos;

namespace ERP.Application.Interfaces
{
    // 倉庫：建立、查全部（先不用分頁，倉庫通常不會很多）
    public interface IWarehouseService
    {
        Task<WarehouseResponse> CreateAsync(CreateWarehouseRequest req, CancellationToken ct = default);
        Task<IReadOnlyList<WarehouseResponse>> GetAllAsync(CancellationToken ct = default);
    }
}
