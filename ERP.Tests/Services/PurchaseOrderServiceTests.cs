using ERP.Application.DTOs;
using ERP.Domain.Entities;
using ERP.Domain.Enums;
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
    * PurchaseOrderService 測試
    */
    public class PurchaseOrderServiceTests
    {
        [Fact]
        public async Task ApproveAsync_Should_Change_Status_From_Draft_To_Approved()
        {
            // Arrange（準備資料）
            await using var db = TestDbContextFactory.Create();

            var supplier = new Supplier
            {
                Code = "S001",
                Name = "測試供應商"
            };

            var product = new Product
            {
                Sku = "SKU001",
                Name = "測試商品",
                Cost = 10,
                Price = 20
            };

            db.Suppliers.Add(supplier);
            db.Products.Add(product);
            await db.SaveChangesAsync();

            var service = new PurchaseOrderService(db);

            // 先建立 PO
            var created = await service.CreateAsync(new CreatePurchaseOrderRequest(
                supplier.Id,
                new List<CreatePurchaseOrderLineRequest>
                {
                new(product.Id, 10, 15)
                }
            ));

            // Act（執行）
            var approved = await service.ApproveAsync(created.Id);

            // Assert（驗證）
            approved.Status.Should().Be(PurchaseOrderStatus.Approved);
            approved.ApprovedAtUtc.Should().NotBeNull();
        }

        [Fact]
        public async Task ApproveAsync_Should_Throw_When_Status_Is_Not_Draft()
        {
            // Arrange
            await using var db = TestDbContextFactory.Create();

            var supplier = new Supplier
            {
                Code = "S001",
                Name = "測試供應商"
            };

            var product = new Product
            {
                Sku = "SKU001",
                Name = "測試商品",
                Cost = 10,
                Price = 20
            };

            db.Suppliers.Add(supplier);
            db.Products.Add(product);
            await db.SaveChangesAsync();

            var service = new PurchaseOrderService(db);

            var created = await service.CreateAsync(new CreatePurchaseOrderRequest(
                supplier.Id,
                new List<CreatePurchaseOrderLineRequest>
                {
                new(product.Id, 10, 15)
                }
            ));

            // 先核准一次
            await service.ApproveAsync(created.Id);

            // Act
            Func<Task> act = async () => await service.ApproveAsync(created.Id);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("*不能核准*");
        }
    }
}
