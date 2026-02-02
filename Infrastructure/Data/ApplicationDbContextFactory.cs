using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace RestaurantOrderTracking.Infrastructure.Data
{
    /// <summary>
    /// Design-time factory for creating ApplicationDbContext instances.
    /// Used by EF Core tools (migrations, scaffolding) when running from CLI.
    /// </summary>
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            // Build configuration from appsettings
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection");

            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

            // Configure PostgreSQL with Npgsql
            optionsBuilder.UseNpgsql(connectionString, npgsqlOptions =>
            {
                // Specify the migrations assembly
                npgsqlOptions.MigrationsAssembly("RestaurantOrderTracking.Infrastructure");
                
                // Enable retry on failure for transient errors
                npgsqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorCodesToAdd: null);
            });

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}
