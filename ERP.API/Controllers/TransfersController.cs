using ERP.Application.DTOs;
using ERP.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ERP.API.Controllers
{
    [ApiController]
    [Route("api/transfers")]
    public class TransfersController : ControllerBase
    {
        private readonly ITransferService _svc;

        public TransfersController(ITransferService svc)
        {
            _svc = svc;
        }

        [HttpPost("新增調撥單")]
        public async Task<ActionResult<TransferResponse>> Create(CreateTransferRequest req, CancellationToken ct)
            => Ok(await _svc.CreateAsync(req, ct));

        [HttpGet("{id:guid}/獲取調撥單明細")]
        public async Task<ActionResult<TransferResponse>> GetById(Guid id, CancellationToken ct)
            => Ok(await _svc.GetByIdAsync(id, ct));

        [HttpGet("查詢")]
        public async Task<ActionResult<IReadOnlyList<TransferResponse>>> GetAll(CancellationToken ct)
            => Ok(await _svc.GetAllAsync(ct));

        [HttpPost("{id:guid}/post/調撥單核准")]
        public async Task<ActionResult<TransferResponse>> Post(Guid id, CancellationToken ct)
            => Ok(await _svc.PostAsync(id, ct));
    }
}
