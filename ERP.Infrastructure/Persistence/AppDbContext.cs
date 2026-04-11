using ERP.Application.Interfaces;
using ERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // 舊寫法（較早期常見）
        //public DbSet<Product> Products { get; set; }
        // 新寫法（較新、較推薦，唯讀且較安全）
        public DbSet<Product> Products => Set<Product>();//商品

        public DbSet<Warehouse> Warehouses => Set<Warehouse>();//倉庫
        public DbSet<StockLedger> StockLedgers => Set<StockLedger>();//庫存流水帳
        public DbSet<InventoryBalance> InventoryBalances => Set<InventoryBalance>();//庫存餘額
        //採購單(PO)
        public DbSet<Supplier> Suppliers => Set<Supplier>();//供應商主檔
        public DbSet<PurchaseOrder> PurchaseOrders => Set<PurchaseOrder>();//採購單（表頭）
        public DbSet<PurchaseOrderLine> PurchaseOrderLines => Set<PurchaseOrderLine>();//採購單明細（表身）
        //收貨單
        public DbSet<GoodsReceipt> GoodsReceipts => Set<GoodsReceipt>();   
        public DbSet<GoodsReceiptLine> GoodsReceiptLines => Set<GoodsReceiptLine>();
        //銷售單(SO)
        public DbSet<Customer> Customers => Set<Customer>();//客戶主檔
        public DbSet<SalesOrder> SalesOrders => Set<SalesOrder>();//銷售單（表頭）
        public DbSet<SalesOrderLine> SalesOrderLines => Set<SalesOrderLine>();//銷售單明細（表身）
        //出貨單
        public DbSet<Shipment> Shipments => Set<Shipment>();
        public DbSet<ShipmentLine> ShipmentLines => Set<ShipmentLine>();
        //調撥
        public DbSet<Transfer> Transfers => Set<Transfer>();
        public DbSet<TransferLine> TransferLines => Set<TransferLine>();
        //登入帳號
        public DbSet<User> Users => Set<User>();



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>()
                .HasIndex(x => x.Sku)
                .IsUnique();

            modelBuilder.Entity<Warehouse>()
                .HasIndex(x => x.Code)
                .IsUnique();

            modelBuilder.Entity<InventoryBalance>()
                .HasIndex(x => new { x.ProductId, x.WarehouseId })
                .IsUnique();

            modelBuilder.Entity<Supplier>()
                .HasIndex(x => x.Code)
                .IsUnique();

            modelBuilder.Entity<PurchaseOrder>()
                .HasIndex(x => x.No)
                .IsUnique();

            // PO 表頭 - 明細 一對多(刪除表頭時連帶所有子表也刪除)
            modelBuilder.Entity<PurchaseOrder>()
                .HasMany(x => x.Lines)
                .WithOne()
                .HasForeignKey(x => x.PurchaseOrderId)//必須有外鍵 沒有表頭的 POId，不能插入明細（避免孤兒資料）
                .OnDelete(DeleteBehavior.Cascade); //Cascade：刪 PO 會刪明細

            modelBuilder.Entity<GoodsReceipt>()
                .HasIndex(x => x.No)
                .IsUnique();

            modelBuilder.Entity<GoodsReceipt>()
                .HasMany(x => x.Lines)
                .WithOne()
                .HasForeignKey(x => x.GoodsReceiptId)
                .OnDelete(DeleteBehavior.Cascade);
            //銷售單表頭 - 明細 一對多(刪除表頭時連帶所有子表也刪除)
            modelBuilder.Entity<Customer>()
                .HasIndex(x => x.Code)
                .IsUnique();

            modelBuilder.Entity<SalesOrder>()
                .HasIndex(x => x.No)
                .IsUnique();

            modelBuilder.Entity<SalesOrder>()
                .HasMany(x => x.Lines)
                .WithOne()
                .HasForeignKey(x => x.SalesOrderId)
                .OnDelete(DeleteBehavior.Cascade);
            //出貨單表頭 - 明細 一對多(刪除表頭時連帶所有子表也刪除)
            modelBuilder.Entity<Shipment>()
                .HasIndex(x => x.No)
                .IsUnique();

            modelBuilder.Entity<Shipment>()
                .HasMany(x => x.Lines)
                .WithOne()
                .HasForeignKey(x => x.ShipmentId)
                .OnDelete(DeleteBehavior.Cascade);
            //調撥單表頭 - 明細 一對多(刪除表頭時連帶所有子表也刪除)
            modelBuilder.Entity<Transfer>()
                .HasIndex(x => x.No)
                .IsUnique();
            //
            modelBuilder.Entity<Transfer>()
                .HasMany(x => x.Lines)
                .WithOne()
                .HasForeignKey(x => x.TransferId)
                .OnDelete(DeleteBehavior.Cascade);
            //
            modelBuilder.Entity<User>()
                .HasIndex(x => x.Username)
                .IsUnique();
        }
    }
}
