using ERP.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Application.Interfaces
{
    public interface IAuthService
    {
        Task RegisterAsync(RegisterRequest req, CancellationToken ct = default);

        Task<LoginResponse> LoginAsync(LoginRequest req, CancellationToken ct = default);
    }
}
