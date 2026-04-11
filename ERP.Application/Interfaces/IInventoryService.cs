using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ERP.Application.DTOs;

namespace ERP.Application.Interfaces
{
 /*
 * IInventoryService = 庫存服務介面（規格）
 */
    public interface IInventoryService
    {
        //新增：入庫
        Task<StockInResponse> StockInAsync(StockInRequest req, CancellationToken ct = default);

        //新增：出庫
        Task<StockOutResponse> StockOutAsync(StockOutRequest req, CancellationToken ct = default);

        //庫存結餘 根據商品id查詢
        Task<IReadOnlyList<InventoryBalanceResponse>> GetBalancesByProductAsync(Guid productId, CancellationToken ct = default);

        //台帳列表項目(每筆詳細 含蒐尋器)
        Task<PagedResult<StockLedgerItemResponse>> GetLedgerAsync(
            Guid? productId,
            Guid? warehouseId,
            string? refNo,
            DateTime? fromUtc,
            DateTime? toUtc,
            int page,
            int pageSize,
            CancellationToken ct = default);


    }
}
