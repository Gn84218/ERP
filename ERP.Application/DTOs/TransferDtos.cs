using ERP.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Application.DTOs
{
    public record CreateTransferLineRequest(
        Guid ProductId,
        decimal Qty
    );

    public record CreateTransferRequest(
        Guid FromWarehouseId,
        Guid ToWarehouseId,
        List<CreateTransferLineRequest> Lines
    );

    public record TransferLineResponse(
        Guid Id,
        Guid ProductId,
        decimal Qty
    );

    public record TransferResponse(
        Guid Id,
        string No,
        Guid FromWarehouseId,
        Guid ToWarehouseId,
        TransferStatus Status,
        DateTime CreatedAtUtc,
        DateTime? PostedAtUtc,
        IReadOnlyList<TransferLineResponse> Lines
    );
}
