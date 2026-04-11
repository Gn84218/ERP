using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Application.DTOs
{
    /*
     * 註冊使用者
     */
    public record RegisterRequest(
        string Username,
        string Password,
        string Role
    );

    /*
     * 登入
     */
    public record LoginRequest(
        string Username,
        string Password
    );

    /*
     * 登入成功回傳
     */
    public record LoginResponse(
        string Token,
        string Username,
        string Role
    );
}
