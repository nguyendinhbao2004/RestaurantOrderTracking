using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RestaurantOrderTracking.Domain.Interface.Repository;
using RestaurantOrderTracking.Infrastructure.Data;
using RestaurantOrderTracking.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace RestaurantOrderTracking.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Cấu hình DbContext với SQL Server
            //services.AddDbContext<ApplicationDbContext>(options =>
            //    options.UseSqlServer(
            //        configuration.GetConnectionString("DefaultConnection"),
            //        sqlOptions => sqlOptions.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)
            //    )
            //);

            // Đăng ký các repository ở đây
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            // Thêm các repository cụ thể khác nếu cần
            return services;
        }
    }
}
