using ERP.Application.DTOs;
using ERP.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ERP.API.Controllers;

[ApiController]
[Route("api/goods-receipts")]
public class GoodsReceiptsController : ControllerBase
{
    private readonly IGoodsReceiptService _svc;
    public GoodsReceiptsController(IGoodsReceiptService svc) => _svc = svc;

    [HttpPost]
    public async Task<ActionResult<GoodsReceiptResponse>> Create(CreateGoodsReceiptRequest req, CancellationToken ct)
        => Ok(await _svc.CreateAsync(req, ct));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<GoodsReceiptResponse>> GetById(Guid id, CancellationToken ct)
        => Ok(await _svc.GetByIdAsync(id, ct));

    //  過帳（會自動入庫）
    [HttpPost("{id:guid}/post")]
    public async Task<ActionResult<GoodsReceiptResponse>> Post(Guid id, CancellationToken ct)
        => Ok(await _svc.PostAsync(id, ct));
}
