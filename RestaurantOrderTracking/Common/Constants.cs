
namespace WebApi.Common
{
    /// <summary>
    /// Centralized application constants.
    /// Structured into nested classes for discoverability and to encourage grouping.
    /// For configuration keys consider using strongly-typed Options instead of string keys.
    /// </summary>
    public static class AppConstants
    {
        /// <summary>Where CORS policy names are stored.</summary>
        public static class Cors
        {
            public const string AllowAll = "AllowAll";
        }

        /// <summary>Connection string keys.</summary>
        public static class ConnectionStrings
        {
            public const string Default = "DefaultConnection";
        }

        /// <summary>Entity Framework / database related settings.</summary>
        public static class Ef
        {
            public const string MigrationsAssembly = "RestaurantOrderTracking.Infrastructure";
        }

        /// <summary>JWT configuration keys.
        /// Prefer binding to a POCO (e.g. JwtSettings) via configuration.GetSection("JwtSettings").Get&lt;JwtSettings&gt;() in production code.</summary>
        public static class Jwt
        {
            public const string Section = "JwtSettings";
            public const string Issuer = "Issuer";
            public const string Audience = "Audience";
            public const string Secret = "SecretKey";
        }

        /// <summary>Swagger / OpenAPI related constants.</summary>
        public static class Swagger
        {
            public const string DocName = "v1";
            public const string Title = "Restaurant Order Tracking API";
            public const string SecurityScheme = "Bearer";
            public const string AuthorizationHeader = "Authorization";
            public const string JwtDescription = "Vui lòng nhập Token vào ô bên dưới (Không cần chữ 'Bearer ' ở đầu)";
        }

        /// <summary>Application-level user-facing messages. Consider localization (resx) for multi-language support.</summary>
        public static class Messages
        {
            public const string Unauthorized = "Bạn chưa đăng nhập hoặc Token không hợp lệ.";
            public const string Forbidden = "Bạn không có quyền truy cập tài nguyên này (Role không đủ).";
        }

        /// <summary>Rate limiting policy names.</summary>
        public static class ConfigConstants
        {
            public const string RateLimitSection = "RateLimitSettings";
            public const string PermitLimit = "PermitLimit";
            public const string WindowInSeconds = "WindowInSeconds";
            public const string QueueLimit = "QueueLimit";
            public const string FixedPolicy = "FixedPolicy";
        }
    }
}

