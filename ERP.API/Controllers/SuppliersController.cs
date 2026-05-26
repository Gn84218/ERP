using ERP.Application.DTOs;
using ERP.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ERP.API.Controllers
{
    //供應商
    [ApiController]
    [Route("api/suppliers")]
    public class SuppliersController : ControllerBase
    {
        private readonly ISupplierService _svc;
        public SuppliersController(ISupplierService svc) => _svc = svc;

        [HttpPost("新增供應商")]
        public async Task<ActionResult<SupplierResponse>> Create(CreateSupplierRequest req, CancellationToken ct)
            => Ok(await _svc.CreateAsync(req, ct));

        [HttpGet("獲取供應商名單")]
        public async Task<ActionResult<IReadOnlyList<SupplierResponse>>> GetAll(CancellationToken ct)
            => Ok(await _svc.GetAllAsync(ct));
    }
}
