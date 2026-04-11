using ERP.Application.DTOs;
using ERP.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ERP.API.Controllers
{
    //供應商 : 創建,獲取所有
    [ApiController]
    [Route("api/suppliers")]
    public class SuppliersController : ControllerBase
    {
        private readonly ISupplierService _svc;
        public SuppliersController(ISupplierService svc) => _svc = svc;

        [HttpPost]
        public async Task<ActionResult<SupplierResponse>> Create(CreateSupplierRequest req, CancellationToken ct)
            => Ok(await _svc.CreateAsync(req, ct));

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<SupplierResponse>>> GetAll(CancellationToken ct)
            => Ok(await _svc.GetAllAsync(ct));
    }
}
