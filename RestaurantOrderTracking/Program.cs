using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using RestaurantOrderTracking.Infrastructure;
using RestaurantOrderTracking.Application;
using RestaurantOrderTracking.Infrastructure.Data;
using RestaurantOrderTracking.WebApi.Middleware;

namespace WebApi
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            // Configure PostgreSQL Database Context
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseNpgsql(
                    builder.Configuration.GetConnectionString("DefaultConnection"),
                    npgsqlOptions =>
                    {
                        npgsqlOptions.MigrationsAssembly("RestaurantOrderTracking.Infrastructure");
                    });
            });

            // Register Database Seeder
            builder.Services.AddDatabaseSeeder();

            builder.Services.AddInfrastructureServices(builder.Configuration);
            builder.Services.AddApplication();

            builder.Services.AddAuthorization();

            builder.Services.AddSwaggerGen(option =>
            {
                option.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "Restaurant Order Tracking API",
                    Version = "v1"
                });
                // 1. Định nghĩa Security Scheme (Cấu hình nút Authorize)
                option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Vui lòng nhập Token vào ô bên dưới (Không cần chữ 'Bearer ' ở đầu)",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });
                // 2. Yêu cầu bảo mật (Áp dụng cho toàn bộ API)
                option.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });
                // Lấy tên file XML theo tên Assembly (Project)
                var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

                // 👇 THÊM ĐOẠN IF NÀY VÀO
                if (File.Exists(xmlPath))
                {
                    option.IncludeXmlComments(xmlPath);
                }
            });


            builder.Services.AddControllers();
            builder.Services.AddOpenApi();
            builder.Services.AddEndpointsApiExplorer(); // ❗ bắt buộc
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Seed database on startup
            await app.Services.SeedDatabaseAsync();

            // Configure the HTTP request pipeline.

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseMiddleware<GlobalExceptionMiddleware>();

            app.UseHttpsRedirection();
            app.UseCors("AllowAll");
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}
