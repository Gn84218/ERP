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

        [HttpPost]
        public async Task<IActionResult> Create(CreateTransferRequest req)
            => Ok(await _svc.CreateAsync(req));

        [HttpPost("{id}/post")]
        public async Task<IActionResult> Post(Guid id)
            => Ok(await _svc.PostAsync(id));
    }
}
