using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace RestaurantOrderTracking.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // 1. Đăng ký AutoMapper (quét toàn bộ assembly này)
            services.AddAutoMapper(typeof(DependencyInjection).Assembly);

            // 2. Đăng ký MediatR (Thay thế cho việc đăng ký từng Service thủ công)
            // Nó sẽ tự tìm tất cả các file Handler để đăng ký
            services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
                // 2. Đăng ký Pipeline Behavior (QUAN TRỌNG)
                // Nó bảo MediatR: "Trước khi chạy Handler, hãy chạy cái ValidationBehavior này trước"
                cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(Common.Behavior.ValidationBehavior<,>));
            });
            return services;
        }
    }
}
