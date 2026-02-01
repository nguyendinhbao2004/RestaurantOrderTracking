using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RestaurantOrderTracking.Domain.Entities;

namespace RestaurantOrderTracking.Infrastructure.Data
{
    public class DatabaseSeeder
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DatabaseSeeder> _logger;

        public DatabaseSeeder(ApplicationDbContext context, ILogger<DatabaseSeeder> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task SeedAsync()
        {
            try
            {
                // Ensure database is created and migrations are applied
                await _context.Database.MigrateAsync();

                // Seed data in order of dependencies
                await SeedRolesAsync();
                await SeedCategoriesAsync();
                await SeedAreasAsync();
                await SeedProductsAsync();

                _logger.LogInformation("Database seeding completed successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while seeding the database.");
                throw;
            }
        }

        private async Task SeedRolesAsync()
        {
            if (await _context.Roles.AnyAsync())
            {
                _logger.LogInformation("Roles already seeded, skipping...");
                return;
            }

            var roles = new List<Role>
            {
                new Role(1, "Admin", "System administrator with full access"),
                new Role(2, "Manager", "Restaurant manager with management access"),
                new Role(3, "Chef", "Kitchen staff responsible for cooking"),
                new Role(4, "Waiter", "Service staff responsible for serving customers"),
                new Role(5, "Cashier", "Staff responsible for handling payments")
            };

            await _context.Roles.AddRangeAsync(roles);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Seeded {Count} roles.", roles.Count);
        }

        private async Task SeedCategoriesAsync()
        {
            if (await _context.Categories.AnyAsync())
            {
                _logger.LogInformation("Categories already seeded, skipping...");
                return;
            }

            var categories = new List<Category>
            {
                new Category(1, "Appetizers"),
                new Category(2, "Main Course"),
                new Category(3, "Desserts"),
                new Category(4, "Beverages"),
                new Category(5, "Soups"),
                new Category(6, "Salads"),
                new Category(7, "Specials")
            };

            await _context.Categories.AddRangeAsync(categories);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Seeded {Count} categories.", categories.Count);
        }

        private async Task SeedAreasAsync()
        {
            if (await _context.Areas.AnyAsync())
            {
                _logger.LogInformation("Areas already seeded, skipping...");
                return;
            }

            var areas = new List<Area>
            {
                new Area("Main Hall"),
                new Area("Outdoor Patio"),
                new Area("Private Room 1"),
                new Area("Private Room 2"),
                new Area("Bar Area"),
                new Area("VIP Section")
            };

            await _context.Areas.AddRangeAsync(areas);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Seeded {Count} areas.", areas.Count);
        }

        private async Task SeedProductsAsync()
        {
            if (await _context.Products.AnyAsync())
            {
                _logger.LogInformation("Products already seeded, skipping...");
                return;
            }

            var products = new List<Product>
            {
                // Appetizers (CategoryId: 1)
                new Product(1, "Spring Rolls", 45000m, true),
                new Product(1, "Crispy Wontons", 38000m, true),
                new Product(1, "Garlic Bread", 25000m, true),
                new Product(1, "Chicken Wings", 65000m, true),

                // Main Course (CategoryId: 2)
                new Product(2, "Grilled Salmon", 185000m, true),
                new Product(2, "Beef Steak", 220000m, true),
                new Product(2, "Chicken Parmesan", 145000m, true),
                new Product(2, "Fried Rice", 55000m, true),
                new Product(2, "Pad Thai", 75000m, true),
                new Product(2, "Pho Bo", 65000m, true),

                // Desserts (CategoryId: 3)
                new Product(3, "Chocolate Cake", 55000m, true),
                new Product(3, "Ice Cream Sundae", 45000m, true),
                new Product(3, "Tiramisu", 65000m, true),
                new Product(3, "Fresh Fruit Platter", 75000m, true),

                // Beverages (CategoryId: 4)
                new Product(4, "Fresh Orange Juice", 35000m, true),
                new Product(4, "Vietnamese Coffee", 28000m, true),
                new Product(4, "Green Tea", 20000m, true),
                new Product(4, "Coca Cola", 18000m, true),
                new Product(4, "Smoothie", 45000m, true),

                // Soups (CategoryId: 5)
                new Product(5, "Tom Yum Soup", 55000m, true),
                new Product(5, "Mushroom Soup", 45000m, true),
                new Product(5, "Chicken Soup", 40000m, true),

                // Salads (CategoryId: 6)
                new Product(6, "Caesar Salad", 65000m, true),
                new Product(6, "Greek Salad", 55000m, true),
                new Product(6, "Garden Salad", 45000m, true),

                // Specials (CategoryId: 7)
                new Product(7, "Chef's Special", 195000m, true),
                new Product(7, "Seafood Platter", 350000m, true)
            };

            await _context.Products.AddRangeAsync(products);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Seeded {Count} products.", products.Count);
        }
    }

    // Extension method for easy registration in Program.cs
    public static class DatabaseSeederExtensions
    {
        public static async Task SeedDatabaseAsync(this IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
            await seeder.SeedAsync();
        }

        public static IServiceCollection AddDatabaseSeeder(this IServiceCollection services)
        {
            services.AddScoped<DatabaseSeeder>();
            return services;
        }
    }
}
