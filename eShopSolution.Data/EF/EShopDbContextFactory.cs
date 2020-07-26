using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace eShopSolution.Data.EF
{
    // vào đây https://docs.microsoft.com/en-us/ef/core/miscellaneous/cli/dbcontext-creation
    public class EShopDbContextFactory : IDesignTimeDbContextFactory<EShopDBContext>
    {
        public EShopDBContext CreateDbContext(string[] args)
        {

            // cấu hình đường dẫn tời file json là appsettings để lấy được ConectionString để kết nối với database
            IConfigurationRoot configuration = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory()) // để có SetBasePath tải thêm Microsoft.Extensions.Configuration.FileExtensions
               .AddJsonFile("appsettings.json") // để có AddJsonFile tải thêm  Microsoft.Extensions.Configuration.Json
               .Build();


            // cấu hình ở trên giờ lấy ra để sử dụng
            var ConectionString = configuration.GetConnectionString("eShopSolutionDb");// đưa tên của ConectionString vào

            var optionsBuilder = new DbContextOptionsBuilder<EShopDBContext>();
            optionsBuilder.UseSqlServer(ConectionString);// chuyền ConectionString vào 

            return new EShopDBContext(optionsBuilder.Options);
        }
    }
}
