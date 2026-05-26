using ERP.Application.DTOs;
using ERP.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

//出貨單
namespace ERP.API.Controllers
{
    [ApiController]
    [Route("api/shipments")]
    public class ShipmentsController : ControllerBase
    {
        private readonly IShipmentService _svc;

        public ShipmentsController(IShipmentService svc)
        {
            _svc = svc;
        }

        [HttpPost("新增出貨單")]
        public async Task<ActionResult<ShipmentResponse>> Create(CreateShipmentRequest req, CancellationToken ct)
            => Ok(await _svc.CreateAsync(req, ct));

        [HttpGet("{id:guid}/出貨單明細")]
        public async Task<ActionResult<ShipmentResponse>> GetById(Guid id, CancellationToken ct)
            => Ok(await _svc.GetByIdAsync(id, ct));

        // Draft -> Shipped，並自動扣庫存
        [HttpPost("{id:guid}/ship/出貨過帳自動扣庫存")]
        public async Task<ActionResult<ShipmentResponse>> Ship(Guid id, CancellationToken ct)
            => Ok(await _svc.ShipAsync(id, ct));
    }
}
