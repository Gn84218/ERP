using ERP.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using static ERP.Application.DTOs.WarehouseDtos;

namespace ERP.API.Controllers
{
    [ApiController]
    // 統一資源路徑：/api/warehouses
    [Route("api/warehouses")]
    public class WarehousesController : ControllerBase
    {
        private readonly IWarehouseService _svc;

        public WarehousesController(IWarehouseService svc) => _svc = svc;

        // POST /api/warehouses
        // Body: { "code": "WH01", "name": "主倉" }
        [HttpPost]
        public async Task<ActionResult<WarehouseResponse>> Create(CreateWarehouseRequest req, CancellationToken ct)
            => Ok(await _svc.CreateAsync(req, ct));

        // GET /api/warehouses
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<WarehouseResponse>>> GetAll(CancellationToken ct)
            => Ok(await _svc.GetAllAsync(ct));
    }
}
