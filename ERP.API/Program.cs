
using System.Text;
using ERP.API.Middlewares;
using ERP.Application.Interfaces;
using ERP.Application.Interfaces.Services;
using ERP.Infrastructure.Persistence;
using ERP.Infrastructure.Security;
using ERP.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace ERP.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // =========================
            // Controllers
            // =========================
            builder.Services.AddControllers();

            // =========================
            // JWT Authentication
            // =========================
            var jwtSettings = builder.Configuration.GetSection("Jwt");
            var jwtKey = jwtSettings["Key"]!;

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateIssuerSigningKey = true,
                        ValidateLifetime = true,

                        ValidIssuer = jwtSettings["Issuer"],
                        ValidAudience = jwtSettings["Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
                        // 消除伺服器與容器之間的時間差干擾
                        ClockSkew = TimeSpan.FromMinutes(5)
                    };
                });

            // =========================
            // SQL Server DbContext
            // =========================
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("Default"),
                sqlServerOptionsAction: sqlOptions =>
                {
                //啟用延遲連線自動重試機制
                sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,               //最多重試 5 次
                maxRetryDelay: TimeSpan.FromSeconds(5), // 每次最多等 5 秒
                errorNumbersToAdd: null);
                }));

            // =========================
            // Redis Cache
            // =========================
            builder.Services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = "erp.redis:6379";
                options.InstanceName = "ERPSystem:";
            });

            // =========================
            // Dependency Injection
            // =========================
            builder.Services.AddScoped<IProductService, ProductService>();               // 商品
            builder.Services.AddScoped<IWarehouseService, WarehouseService>();           // 倉庫
            builder.Services.AddScoped<ISupplierService, SupplierService>();             // 供應商
            builder.Services.AddScoped<IPurchaseOrderService, PurchaseOrderService>();   // 採購單
            builder.Services.AddScoped<IInventoryService, InventoryService>();           // 庫存服務
            builder.Services.AddScoped<IGoodsReceiptService, GoodsReceiptService>();     // 收貨單
            builder.Services.AddScoped<ISalesOrderService, SalesOrderService>();         // 銷售單
            builder.Services.AddScoped<ICustomerService, CustomerService>();             // 客戶
            builder.Services.AddScoped<IShipmentService, ShipmentService>();             // 出貨單
            builder.Services.AddScoped<ITransferService, TransferService>();             // 調撥單
            builder.Services.AddScoped<JwtTokenGenerator>();                             // JWT 產生器
            builder.Services.AddScoped<IAuthService, AuthService>();                     // 認證服務
            //Docker
            //builder.WebHost.UseUrls("http://0.0.0.0:8080");

            // =========================
            // Swagger
            // =========================
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                // 定義 Bearer Token 的 Swagger 安全性描述
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "請輸入 JWT Token。格式：Bearer {your token}"
                });

                // 套用到所有受保護 API
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            var app = builder.Build();

            

            // =========================
            // Swagger UI
            // =========================
           // if (app.Environment.IsDevelopment())
           // {
                app.UseSwagger();
                app.UseSwaggerUI();
            //  }

            // =========================
            // Global Exception Middleware
            // =========================
            app.UseMiddleware<ExceptionHandlingMiddleware>();

            // =========================
            // HTTP Pipeline
            // =========================
            //!!!! app.UseHttpsRedirection();
            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseAuthentication();   // 先驗證
            app.UseAuthorization();    // 再授權

            app.MapControllers();

            // 自動套用 Migration 的程式碼
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<AppDbContext>();

                    //自動在 Docker 空白資料庫中，建立所有資料表
                    context.Database.Migrate();
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "在自動轉移資料庫時發生錯誤！");
                }
            }

            app.Run(); // 原本的啟動行

            app.Run();
        }
    }
}
