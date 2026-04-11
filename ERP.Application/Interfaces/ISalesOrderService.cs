using ERP.Application.DTOs;

namespace ERP.Application.Interfaces;

/*
 * 銷售單服務介面
 * 功能：
 * 1. 建立銷售單
 * 2. 查單筆銷售單
 * 3. 核准銷售單（Draft -> Approved）
 */
public interface ISalesOrderService
{
    Task<SalesOrderResponse> CreateAsync(CreateSalesOrderRequest req, CancellationToken ct = default);

    Task<SalesOrderResponse> GetByIdAsync(Guid id, CancellationToken ct = default);

    Task<SalesOrderResponse> ApproveAsync(Guid id, CancellationToken ct = default);
}