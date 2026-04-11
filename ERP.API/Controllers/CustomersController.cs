using ERP.Application.DTOs;
using ERP.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

//顧客
namespace ERP.API.Controllers
{
    [ApiController]
    [Route("api/customers")]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService _svc;

        public CustomersController(ICustomerService svc)
        {
            _svc = svc;
        }

        [HttpPost]
        public async Task<ActionResult<CustomerResponse>> Create(CreateCustomerRequest req, CancellationToken ct)
            => Ok(await _svc.CreateAsync(req, ct));

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<CustomerResponse>>> GetAll(CancellationToken ct)
            => Ok(await _svc.GetAllAsync(ct));
    }
}
