using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace RestaurantOrderTracking.Infrastructure.Data
{
    //public class ContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    //{
    //    public ApplicationDbContext CreateDbContext(string[] args)
    //    {
    //        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

    //        // Thay chuỗi kết nối của bạn vào đây (Copy từ appsettings.json)
    //        var configuration = new ConfigurationBuilder()
    //            .AddJsonFile("appsettings.json", optional: false)
    //            .AddJsonFile("appsettings.Development.json", optional: true)
    //            .AddEnvironmentVariables()
    //            .Build();

    //        var connectionString =
    //            configuration.GetConnectionString("DefaultConnection");

    //        optionsBuilder.UseNpgsql(connectionString, o =>
    //        {
    //            // Chỉ định migration nằm ngay tại project này
    //            o.MigrationsAssembly("ChatBotInterfacture");

    //            // --- CHÌA KHÓA ĐỂ SỬA LỖI ---
    //            o.UseVector();
    //        });

    //        return new ApplicationDbContext(optionsBuilder.Options);
    //    }
    //}
}
