using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RestaurantOrderTracking.Application;
using RestaurantOrderTracking.Domain.Common;
using RestaurantOrderTracking.Infrastructure;
using RestaurantOrderTracking.Infrastructure.Data;
using RestaurantOrderTracking.WebApi.Hubs;
using RestaurantOrderTracking.WebApi.Extensions;
using RestaurantOrderTracking.WebApi.Middleware;
using System.Text;
using System.Text.Json;
using WebApi.Common;

namespace WebApi
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            // Load .env file for environment variables

            //test
            DotNetEnv.Env.Load();
            var builder = WebApplication.CreateBuilder(args);
            builder.Configuration.AddEnvironmentVariables();
            var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy(AppConstants.Cors.AllowAll, policy =>
                {
                    if (allowedOrigins.Length > 0)
                    {
                        policy.WithOrigins(allowedOrigins)
                              .AllowAnyMethod()
                              .AllowAnyHeader()
                              .AllowCredentials();
                    }
                    else
                    {
                        policy.AllowAnyOrigin()
                              .AllowAnyMethod()
                              .AllowAnyHeader();
                    }
                });
            });

            //add ratelimit
            builder.Services.AddCustomRateLimiter(builder.Configuration);

            // Configure PostgreSQL Database Context
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseNpgsql(
                    builder.Configuration.GetConnectionString(AppConstants.ConnectionStrings.Default),
                    npgsqlOptions =>
                    {
                        npgsqlOptions.MigrationsAssembly(AppConstants.Ef.MigrationsAssembly);
                    });
            });

            // Register Database Seeder
            builder.Services.AddDatabaseSeeder();

            builder.Services.AddInfrastructureServices(builder.Configuration);
            builder.Services.AddApplication();
            builder.Services.AddSignalR();
            builder.Services.AddScoped<IHubContext>(sp => (IHubContext)sp.GetRequiredService<IHubContext<RestaurantHub>>());

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                //TokenValidationParameters
                var jwtSettings = builder.Configuration.GetSection(AppConstants.Jwt.Section);
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings[AppConstants.Jwt.Issuer],
                    ValidAudience = jwtSettings[AppConstants.Jwt.Audience],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings[AppConstants.Jwt.Secret]))
                };

                //CUSTOM TRẢ VỀ JSON CHO 401 & 403
                options.Events = new JwtBearerEvents
                {
                    // help SignalR hub to get token from query string when connecting
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];
                        var path = context.HttpContext.Request.Path;

                        if (!string.IsNullOrWhiteSpace(accessToken)
                            && path.StartsWithSegments("/hubs/restaurant"))
                        {
                            context.Token = accessToken;
                        }

                        return Task.CompletedTask;
                    },

                    // 1. Xử lý khi chưa đăng nhập (401 Unauthorized)
                    OnChallenge = context =>
                    {
                        context.HandleResponse();

                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        context.Response.ContentType = "application/json";

                        var result = Result.Failure(AppConstants.Messages.Unauthorized);
                        var json = JsonSerializer.Serialize(result);

                        return context.Response.WriteAsync(json);
                    },

                    // 2. Xử lý khi đăng nhập rồi nhưng không đủ quyền (403 Forbidden)
                    OnForbidden = context =>
                    {
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        context.Response.ContentType = "application/json";

                        var result = Result.Failure(AppConstants.Messages.Forbidden);
                        var json = JsonSerializer.Serialize(result);

                        return context.Response.WriteAsync(json);
                    }
                };
            });

            builder.Services.AddAuthorization();

            builder.Services.AddSwaggerGen(option =>
            {
                option.SwaggerDoc(AppConstants.Swagger.DocName, new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = AppConstants.Swagger.Title,
                    Version = AppConstants.Swagger.DocName
                });
                // 1. Định nghĩa Security Scheme (Cấu hình nút Authorize)
                option.AddSecurityDefinition(AppConstants.Swagger.SecurityScheme, new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = AppConstants.Swagger.JwtDescription,
                    Name = AppConstants.Swagger.AuthorizationHeader,
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = AppConstants.Swagger.SecurityScheme
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
                                Id = AppConstants.Swagger.SecurityScheme
                            }
                        },
                        new string[] {}
                    }
                });
                // Lấy tên file XML theo tên Assembly (Project)
                var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

                
                if (File.Exists(xmlPath))
                {
                    option.IncludeXmlComments(xmlPath);
                }
            });


            builder.Services.AddControllers();
            builder.Services.AddOpenApi();
            builder.Services.AddEndpointsApiExplorer(); 
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
            app.UseRateLimiter();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            app.MapHub<RestaurantHub>("/hubs/restaurant");

            app.Run();
        }
    }
}
