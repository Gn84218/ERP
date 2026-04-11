using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ERP.Application.DTOs.ProductDtos;


namespace ERP.Application.Interfaces
{
    // 這裡只定「能做什麼」：建立商品、分頁查商品
    public interface IProductService
    {
        Task<ProductResponse> CreateAsync(CreateProductRequest req, CancellationToken ct = default);

        // page/pageSize 先做簡易分頁（後面可加 TotalCount）
        Task<IReadOnlyList<ProductResponse>> GetPagedAsync(int page, int pageSize, CancellationToken ct = default);
    }
}
