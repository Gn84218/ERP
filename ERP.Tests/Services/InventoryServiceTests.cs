using ERP.Application.DTOs;
using ERP.Domain.Entities;
using ERP.Infrastructure.Services;
using ERP.Tests.Helpers;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Tests.Services
{
    /*
     * InventoryService 測試
     */
    public class InventoryServiceTests
    {
        [Fact]
        public async Task StockOutAsync_Should_Throw_When_Stock_Is_Insufficient()
        {
            // Arrange
            await using var db = TestDbContextFactory.Create();

            var product = new Product
            {
                Sku = "SKU001",
                Name = "測試商品",
                Cost = 10,
                Price = 20
            };

            var warehouse = new Warehouse
            {
                Code = "WH01",
                Name = "主倉"
            };

            // 目前只有 5 庫存
            var balance = new InventoryBalance
            {
                ProductId = product.Id,
                WarehouseId = warehouse.Id,
                OnHandQty = 5,
                UpdatedAtUtc = DateTime.UtcNow
            };

            db.Products.Add(product);
            db.Warehouses.Add(warehouse);
            db.InventoryBalances.Add(balance);
            await db.SaveChangesAsync();

            var service = new InventoryService(db);

            // Act
            Func<Task> act = async () => await service.StockOutAsync(new StockOutRequest(
                ProductId: product.Id,
                WarehouseId: warehouse.Id,
                Qty: 10, // 想出 10，但只有 5
                RefType: "SO",
                RefNo: "SO0001",
                Remark: "測試庫存不足"
            ));

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("*庫存不足*");
        }

        [Fact]
        public async Task StockOutAsync_Should_Decrease_Balance_When_Stock_Is_Enough()
        {
            // Arrange
            await using var db = TestDbContextFactory.Create();

            var product = new Product
            {
                Sku = "SKU001",
                Name = "測試商品",
                Cost = 10,
                Price = 20
            };

            var warehouse = new Warehouse
            {
                Code = "WH01",
                Name = "主倉"
            };

            var balance = new InventoryBalance
            {
                ProductId = product.Id,
                WarehouseId = warehouse.Id,
                OnHandQty = 10,
                UpdatedAtUtc = DateTime.UtcNow
            };

            db.Products.Add(product);
            db.Warehouses.Add(warehouse);
            db.InventoryBalances.Add(balance);
            await db.SaveChangesAsync();

            var service = new InventoryService(db);

            // Act
            var result = await service.StockOutAsync(new StockOutRequest(
                ProductId: product.Id,
                WarehouseId: warehouse.Id,
                Qty: 4,
                RefType: "SO",
                RefNo: "SO0001",
                Remark: "正常出庫"
            ));

            // Assert
            result.Qty.Should().Be(-4);

            var updatedBalance = db.InventoryBalances.Single(x =>
                x.ProductId == product.Id && x.WarehouseId == warehouse.Id);

            updatedBalance.OnHandQty.Should().Be(6);
        }
    }
}
