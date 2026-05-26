using ERP.Application.DTOs;
using ERP.Application.Interfaces;
using ERP.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace ERP.API.Controllers
{
    [ApiController]
    [Route("api/sales-orders")]
    public class SalesOrdersController : ControllerBase
    {
        private readonly ISalesOrderService _svc;

        public SalesOrdersController(ISalesOrderService svc)
        {
            _svc = svc;
        }

        [HttpPost("新增銷售單")]
        public async Task<ActionResult<SalesOrderResponse>> Create(CreateSalesOrderRequest req, CancellationToken ct)
        => Ok(await _svc.CreateAsync(req, ct));

        [HttpGet("{id:guid}/銷售單明細")]
        public async Task<ActionResult<SalesOrderResponse>> GetById(Guid id, CancellationToken ct)
            => Ok(await _svc.GetByIdAsync(id, ct));

        [HttpGet("查詢")]
        public async Task<ActionResult<IReadOnlyList<SalesOrderResponse>>> GetAll(CancellationToken ct)
            => Ok(await _svc.GetAllAsync(ct));

        [HttpPost("{id:guid}/approve/核准")]
        public async Task<ActionResult<SalesOrderResponse>> Approve(Guid id, CancellationToken ct)
            => Ok(await _svc.ApproveAsync(id, ct));
    }
}
