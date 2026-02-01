using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace RestaurantOrderTracking.Infrastructure.Data
{
    public class ContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
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
            });

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}
