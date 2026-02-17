using Microsoft.AspNetCore.RateLimiting;
using System.Net.Mime;
using System.Threading.RateLimiting;
using WebApi.Common;

namespace RestaurantOrderTracking.WebApi.Extensions
{
    public static class RateLimiterExtensions
    {
        public static IServiceCollection AddCustomRateLimiter(this IServiceCollection services, IConfiguration config)
        {
            //truy cập section 
            var rateLimitSection = config.GetSection(AppConstants.ConfigConstants.RateLimitSection);
            // Đọc giá trị với fallback (giá trị mặc định) để tránh lỗi runtime
            var permitLimit = rateLimitSection.GetValue<int>(AppConstants.ConfigConstants.PermitLimit, 100);
            var windowInSeconds = rateLimitSection.GetValue<int>(AppConstants.ConfigConstants.WindowInSeconds, 60);
            var queueLimit = rateLimitSection.GetValue<int>(AppConstants.ConfigConstants.QueueLimit, 2);

            services.AddRateLimiter(async options =>
            {
                options.AddFixedWindowLimiter(policyName: AppConstants.ConfigConstants.FixedPolicy, opt =>
                {
                    opt.PermitLimit = permitLimit;
                    opt.Window = TimeSpan.FromSeconds(windowInSeconds);
                    opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                    opt.QueueLimit = queueLimit;
                });

                options.OnRejected = async (context, token) =>
                {
                    context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                    context.HttpContext.Response.ContentType = MediaTypeNames.Application.Json;
                    var result = new
                    {
                        status = false,
                        message = "Too many requests. Please try again later."
                    };
                    await context.HttpContext.Response.WriteAsJsonAsync(result, cancellationToken: token);
                };
            });
            return services;
        }
    }
}
