using ERP.Application.DTOs;
using ERP.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ERP.API.Controllers;
//收據單
[ApiController]
[Route("api/goods-receipts")]
public class GoodsReceiptsController : ControllerBase
{
    private readonly IGoodsReceiptService _svc;
    public GoodsReceiptsController(IGoodsReceiptService svc) => _svc = svc;

    [HttpPost("建立收據單(表+明細)")]
    public async Task<ActionResult<GoodsReceiptResponse>> Create(CreateGoodsReceiptRequest req, CancellationToken ct)
        => Ok(await _svc.CreateAsync(req, ct));

    [HttpGet("{id:guid}/獲取明細")]
    public async Task<ActionResult<GoodsReceiptResponse>> GetById(Guid id, CancellationToken ct)
        => Ok(await _svc.GetByIdAsync(id, ct));

    [HttpGet("查詢")]
    public async Task<ActionResult<IReadOnlyList<GoodsReceiptResponse>>> GetAll(CancellationToken ct)
        => Ok(await _svc.GetAllAsync(ct));

    //  過帳（會自動入庫）
    [HttpPost("{id:guid}/post/過帳(自動入庫)")]
    public async Task<ActionResult<GoodsReceiptResponse>> Post(Guid id, CancellationToken ct)
        => Ok(await _svc.PostAsync(id, ct));
}
