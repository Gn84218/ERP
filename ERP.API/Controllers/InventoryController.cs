using ERP.Application.DTOs;
using ERP.Application.Interfaces;
using ERP.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace ERP.API.Controllers
{
    /*
 * InventoryController = 庫存控制器
 * 提供 HTTP API
 * 路徑規劃：
 * POST /api/inventory/stock-in  → 入庫
 */
    [ApiController]
    [Route("api/inventory")]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;
        public InventoryController(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        //  新增：入庫 API
        [HttpPost("stock-in")]
        public async Task<ActionResult<StockInResponse>> StockIn(StockInRequest req, CancellationToken ct)
        {
            var result = await _inventoryService.StockInAsync(req, ct);
            return Ok(result);
        }

        //  新增：出庫 API
        [HttpPost("stock-out")]
        public async Task<ActionResult<StockOutResponse>> StockOut(StockOutRequest req, CancellationToken ct)
        {
            var result = await _inventoryService.StockOutAsync(req, ct);
            return Ok(result);
        }

        //庫存結餘 根據商品ID
        [HttpGet("balances")]
        public async Task<ActionResult<InventoryBalanceResponse>> GetBalances([FromQuery] Guid productId, CancellationToken ct)
        {
            var result=await _inventoryService.GetBalancesByProductAsync(productId, ct);
            return Ok(result);  
        }

        //查庫存台帳 分頁+搜尋關鍵字
        [HttpGet("ledger")]
         public async Task<ActionResult<PagedResult<StockLedgerItemResponse>>> GetLedger(
         [FromQuery] Guid? productId,
         [FromQuery] Guid? warehouseId,
         [FromQuery] string? refNo,
         [FromQuery] DateTime? fromUtc,
         [FromQuery] DateTime? toUtc,
         [FromQuery] int page = 1,
         [FromQuery] int pageSize = 20,CancellationToken ct = default)
         {
            var result = await _inventoryService.GetLedgerAsync(productId, warehouseId, refNo, fromUtc, toUtc, page, pageSize, ct);
            return Ok(result);
         }

}
}
