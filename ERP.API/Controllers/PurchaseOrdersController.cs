using ERP.Application.DTOs;
using ERP.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

//
namespace ERP.API.Controllers
{
    [ApiController]
    [Route("api/purchase-orders")]
    public class PurchaseOrdersController:ControllerBase
    {
        private readonly IPurchaseOrderService _svc;
        public PurchaseOrdersController(IPurchaseOrderService svc) => _svc = svc;

        [HttpPost]
        public async Task<ActionResult<PurchaseOrderResponse>> Create(CreatePurchaseOrderRequest req, CancellationToken ct)
            => Ok(await _svc.CreateAsync(req, ct));

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<PurchaseOrderResponse>> GetById(Guid id, CancellationToken ct)
            => Ok(await _svc.GetByIdAsync(id, ct));

        // 核准：草稿(Draft) → 認可(Approved)
        [HttpPost("{id:guid}/approve")]
        public async Task<ActionResult<PurchaseOrderResponse>> Approve(Guid id, CancellationToken ct)
            => Ok(await _svc.ApproveAsync(id, ct));
    }
}
