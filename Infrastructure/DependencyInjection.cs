using Domain.Interface.Repository;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RestaurantOrderTracking.Domain.Entities;
using RestaurantOrderTracking.Domain.Interface;
using RestaurantOrderTracking.Domain.Interface.Repository;
using RestaurantOrderTracking.Infrastructure.Authentication;
using RestaurantOrderTracking.Infrastructure.Data;
using RestaurantOrderTracking.Infrastructure.Repositories;
using RestaurantOrderTracking.Infrastructure.Services;
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
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
            services.AddScoped<ITableRepository, TableRepository>();
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IOrderItemRepository, OrderItemRepository>();
            services.AddScoped<IOrderItemLogRepository, OrderItemLogRepository>();
            services.AddScoped<IBillRepository, BillRepository>();
            services.AddScoped<IWaiterRepository, WaiterRepository>();
            services.AddScoped<IQRSessionRepository, QRSessionRepository>();

            // QR Code Service
            services.AddScoped<RestaurantOrderTracking.Domain.Interface.IQRCodeService, QRCodeService>();
            services.AddScoped<IWorkScheduleRepository, WorkScheduleRepository>();

            return services;
        }
    }
}
