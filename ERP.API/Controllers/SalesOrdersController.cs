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

        [HttpPost]
        public async Task<ActionResult<SalesOrderResponse>> Create(CreateSalesOrderRequest req, CancellationToken ct)
        => Ok(await _svc.CreateAsync(req, ct));

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<SalesOrderResponse>> GetById(Guid id, CancellationToken ct)
            => Ok(await _svc.GetByIdAsync(id, ct));

        [HttpPost("{id:guid}/approve")]
        public async Task<ActionResult<SalesOrderResponse>> Approve(Guid id, CancellationToken ct)
            => Ok(await _svc.ApproveAsync(id, ct));
    }
}
