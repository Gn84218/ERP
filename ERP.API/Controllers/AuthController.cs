using ERP.Application.DTOs;
using ERP.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ERP.API.Controllers;
//身分驗證 : 註冊+登入
[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _svc;

    public AuthController(IAuthService svc)
    {
        _svc = svc;
    }
    //註冊帳號
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest req, CancellationToken ct)
    {
        await _svc.RegisterAsync(req, ct);
        return Ok(new { message = "註冊成功" });
    }
    //登入帳號
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login(LoginRequest req, CancellationToken ct)
        => Ok(await _svc.LoginAsync(req, ct));
}