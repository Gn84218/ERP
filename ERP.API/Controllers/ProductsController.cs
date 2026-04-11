using ERP.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using static ERP.Application.DTOs.ProductDtos;
using Microsoft.AspNetCore.Authorization;
//商品
namespace ERP.API.Controllers
{
    [Authorize]
    [ApiController]
    // 統一資源路徑：/api/products
    [Route("api/products")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _svc;

        public ProductsController(IProductService svc) => _svc = svc;

        // POST /api/products
        // Body: { "sku": "...", "name": "...", "cost": 10, "price": 20 }
        [Authorize(Roles ="Admin")]
        [HttpPost]
        public async Task<ActionResult<ProductResponse>> Create(CreateProductRequest req, CancellationToken ct)
            => Ok(await _svc.CreateAsync(req, ct));

        // GET /api/products?page=1&pageSize=20
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<ProductResponse>>> GetPaged(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            CancellationToken ct = default)
            => Ok(await _svc.GetPagedAsync(page, pageSize, ct));
    }
}
