namespace ERP.Application.DTOs;

/*
 * 建立客戶 Request
 */
public record CreateCustomerRequest(
    string Code,
    string Name
);

/*
 * 客戶 Response
 */
public record CustomerResponse(
    Guid Id,
    string Code,
    string Name,
    bool IsActive
);