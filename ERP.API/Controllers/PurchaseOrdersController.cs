using ERP.Application.DTOs;
using ERP.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

// 採購單
namespace ERP.API.Controllers
{
    [ApiController]
    [Route("api/purchase-orders")]
    public class PurchaseOrdersController:ControllerBase
    {
        private readonly IPurchaseOrderService _svc;
        public PurchaseOrdersController(IPurchaseOrderService svc) => _svc = svc;

        [HttpPost("新增採購單")]
        public async Task<ActionResult<PurchaseOrderResponse>> Create(CreatePurchaseOrderRequest req, CancellationToken ct)
            => Ok(await _svc.CreateAsync(req, ct));

        [HttpGet("{id:guid}/獲取採購單明細")]
        public async Task<ActionResult<PurchaseOrderResponse>> GetById(Guid id, CancellationToken ct)
            => Ok(await _svc.GetByIdAsync(id, ct));

        [HttpGet("查詢")]
        public async Task<ActionResult<IReadOnlyList<PurchaseOrderResponse>>> GetAll(CancellationToken ct)
            => Ok(await _svc.GetAllAsync(ct));

        // 核准：草稿(Draft) → 認可(Approved)
        [HttpPost("{id:guid}/approve/核准")]
        public async Task<ActionResult<PurchaseOrderResponse>> Approve(Guid id, CancellationToken ct)
            => Ok(await _svc.ApproveAsync(id, ct));
    }
}
