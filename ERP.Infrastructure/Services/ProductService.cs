using ERP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ERP.Application.DTOs.ProductDtos;
using ERP.Application.DTOs;
using ERP.Application.Interfaces;
using ERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using ERP.Application.Interfaces;
using ERP.Domain.Entities;
using ERP.Infrastructure.Persistence;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace ERP.Application.Interfaces.Services
{
    // 建立商品、分頁查商品 !!使用Redis
    public class ProductService : IProductService
    {
        private readonly AppDbContext _db;
        private readonly IDistributedCache _cache;

        // public ProductService(AppDbContext db) => _db = db;
        public ProductService(AppDbContext db, IDistributedCache cache) 
        {
            _db= db;
            _cache = cache;
        }
        //創建
        public async Task<ProductResponse> CreateAsync(CreateProductRequest req, CancellationToken ct = default)
        {
            //去空白
            var sku = req.Sku.Trim();

            // SKU 唯一 如果已存在就丟例外
            var exists = await _db.Products.AnyAsync(x => x.Sku == sku, ct);
            if (exists) throw new InvalidOperationException($"SKU 已存在：{sku}");

            // 新建數據魔形
            var entity = new Product
            {
                Sku = sku,
                Name = req.Name.Trim(),
                Cost = req.Cost,
                Price = req.Price,
                IsActive = true
            };

            _db.Products.Add(entity);
            await _db.SaveChangesAsync(ct);
            //刪除舊得快取
            await _cache.RemoveAsync("products:page:1:size:20", ct);
            // 回傳 DTO：避免直接把 Entity 暴露出去
            return new ProductResponse(entity.Id, entity.Sku, entity.Name, entity.Cost, entity.Price, entity.IsActive);
        }
        //分頁查詢
        public async Task<IReadOnlyList<ProductResponse>> GetPagedAsync(int page, int pageSize, CancellationToken ct = default)
        {
            // 防呆：避免 page=0 或 pageSize 太大
            page = page <= 0 ? 1 : page;
            pageSize = pageSize is < 1 or > 200 ? 20 : pageSize;

            /*
             * 每一組查詢條件的快取 key
             * page=1,pageSize=20
             * 對應命名 products:page:1:size:20
             */
            var cacheKey = $"products:page:{page}:size:{pageSize}";

            // 1. 先查 Redis是否已有快取
            var cachedJson = await _cache.GetStringAsync(cacheKey, ct);

            if (!string.IsNullOrWhiteSpace(cachedJson))
            {
                var cachedData = JsonSerializer.Deserialize<List<ProductResponse>>(cachedJson);//反序列化 (JSON格式轉回List)

                if (cachedData != null)
                {
                    return cachedData;
                }
            }
            //2.沒快取再 查詢：排序 + Skip/Take（分頁）
            var data= await _db.Products
                .OrderBy(x => x.Sku)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new ProductResponse(x.Id, x.Sku, x.Name, x.Cost, x.Price, x.IsActive))
                .ToListAsync(ct);
            // 3.查到資料後寫回 Redis
            // 3-1.定時10分鐘
            var options =new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10) // 快取 10 分鐘
            };
            //3-2.存入快取 (LIST轉JOSN格式)
            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(data), options, ct);//序列化(LIST轉JOSN)
            return data;
        }
    }
}
