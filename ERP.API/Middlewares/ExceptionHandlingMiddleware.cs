using System.Net;
using System.Text.Json;

namespace ERP.API.Middlewares
{
 /*
 * 全域錯誤處理 Middleware
 * 作用：
 * 1. 攔截整個 API pipeline 裡發生的例外
 * 2. 回傳統一格式 JSON
 * 3. 避免直接把系統錯誤細節噴給前端
 */
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(
            RequestDelegate next,
            ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // 放行給下一層（Controller / Service）
                await _next(context);
            }
            catch (Exception ex)
            {
                // 記錄錯誤
                _logger.LogError(ex, "發生未處理例外");

                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            // 預設：系統錯誤
            var statusCode = (int)HttpStatusCode.InternalServerError;
            var message = "系統發生未預期錯誤。";

            // 目前大多數錯誤都是 InvalidOperationException
            if (ex is InvalidOperationException)
            {
                statusCode = (int)HttpStatusCode.BadRequest;
                message = ex.Message;
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            var response = new
            {
                status = statusCode,
                error = message
            };

            var json = JsonSerializer.Serialize(response);
            await context.Response.WriteAsync(json);
        }
    }
}
