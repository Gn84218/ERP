
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ERP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
namespace ERP.Tests.Helpers
{
    /*
    * 測試用 DbContext 工廠
    * 每次測試都建立一個新的 InMemory 資料庫
    * 避免不同測試互相污染資料
    */
    public static class TestDbContextFactory
    {
        public static AppDbContext Create()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()) // 每次都不同 DB 名稱
                .Options;

            return new AppDbContext(options);
        }
    }
}
