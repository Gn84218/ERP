using ERP.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Application.Interfaces
{
    public interface IGoodsReceiptService
    {
        Task<GoodsReceiptResponse> CreateAsync(CreateGoodsReceiptRequest req, CancellationToken ct = default);
        Task<GoodsReceiptResponse> GetByIdAsync(Guid id, CancellationToken ct = default);

        //  過帳：Draft → Posted，並且「自動入庫」
        Task<GoodsReceiptResponse> PostAsync(Guid id, CancellationToken ct = default);
    }
}
