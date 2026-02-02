using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RestaurantOrderTracking.Domain.Entities;
using RestaurantOrderTracking.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantOrderTracking.Infrastructure.Data
{
    /// <summary>
    /// Database seeder for initializing the database with required seed data.
    /// </summary>
    public class DatabaseSeeder
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DatabaseSeeder> _logger;

        public DatabaseSeeder(ApplicationDbContext context, ILogger<DatabaseSeeder> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Seeds the database with initial data. Should be called during application startup.
        /// </summary>
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
                await SeedTablesAsync();

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
                new Category(1, "Appetizers", "Light dishes to start your meal"),
                new Category(2, "Main Course", "Hearty main dishes"),
                new Category(3, "Desserts", "Sweet treats to end your meal"),
                new Category(4, "Beverages", "Refreshing drinks"),
                new Category(5, "Soups", "Warm and comforting soups"),
                new Category(6, "Salads", "Fresh and healthy salads"),
                new Category(7, "Specials", "Chef's special recommendations")
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
                new Area("Main Hall", "The main dining area"),
                new Area("Outdoor Patio", "Open-air dining space"),
                new Area("Private Room 1", "Small private dining room"),
                new Area("Private Room 2", "Medium private dining room"),
                new Area("Bar Area", "Casual bar seating"),
                new Area("VIP Section", "Premium dining experience")
            };

            await _context.Areas.AddRangeAsync(areas);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Seeded {Count} areas.", areas.Count);
        }

        private async Task SeedTablesAsync()
        {
            if (await _context.Tables.AnyAsync())
            {
                _logger.LogInformation("Tables already seeded, skipping...");
                return;
            }

            var areas = await _context.Areas.ToListAsync();
            var mainHallId = areas.FirstOrDefault(a => a.Name == "Main Hall")?.Id ?? Guid.Empty;
            var patioId = areas.FirstOrDefault(a => a.Name == "Outdoor Patio")?.Id ?? Guid.Empty;
            var barId = areas.FirstOrDefault(a => a.Name == "Bar Area")?.Id ?? Guid.Empty;

            if (mainHallId == Guid.Empty || patioId == Guid.Empty || barId == Guid.Empty)
            {
                _logger.LogWarning("Required areas not found, skipping table seeding...");
                return;
            }

            var tables = new List<Table>
            {
                // Main Hall tables
                new Table("T1", mainHallId, 4, TableStatus.Available),
                new Table("T2", mainHallId, 4, TableStatus.Available),
                new Table("T3", mainHallId, 6, TableStatus.Available),
                new Table("T4", mainHallId, 6, TableStatus.Available),
                new Table("T5", mainHallId, 8, TableStatus.Available),
                
                // Outdoor Patio tables
                new Table("P1", patioId, 4, TableStatus.Available),
                new Table("P2", patioId, 4, TableStatus.Available),
                new Table("P3", patioId, 6, TableStatus.Available),
                
                // Bar Area tables
                new Table("B1", barId, 2, TableStatus.Available),
                new Table("B2", barId, 2, TableStatus.Available),
                new Table("B3", barId, 4, TableStatus.Available)
            };

            await _context.Tables.AddRangeAsync(tables);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Seeded {Count} tables.", tables.Count);
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
                new Product(1, "Spring Rolls", 45000m, true, "Crispy vegetable spring rolls"),
                new Product(1, "Crispy Wontons", 38000m, true, "Golden fried wontons with dipping sauce"),
                new Product(1, "Garlic Bread", 25000m, true, "Toasted bread with garlic butter"),
                new Product(1, "Chicken Wings", 65000m, true, "Spicy buffalo chicken wings"),

                // Main Course (CategoryId: 2)
                new Product(2, "Grilled Salmon", 185000m, true, "Atlantic salmon with herbs"),
                new Product(2, "Beef Steak", 220000m, true, "Premium beef steak cooked to perfection"),
                new Product(2, "Chicken Parmesan", 145000m, true, "Breaded chicken with marinara sauce"),
                new Product(2, "Fried Rice", 55000m, true, "Wok-fried rice with vegetables"),
                new Product(2, "Pad Thai", 75000m, true, "Thai stir-fried rice noodles"),
                new Product(2, "Pho Bo", 65000m, true, "Vietnamese beef noodle soup"),

                // Desserts (CategoryId: 3)
                new Product(3, "Chocolate Cake", 55000m, true, "Rich chocolate layer cake"),
                new Product(3, "Ice Cream Sundae", 45000m, true, "Vanilla ice cream with toppings"),
                new Product(3, "Tiramisu", 65000m, true, "Italian coffee-flavored dessert"),
                new Product(3, "Fresh Fruit Platter", 75000m, true, "Seasonal fresh fruits"),

                // Beverages (CategoryId: 4)
                new Product(4, "Fresh Orange Juice", 35000m, true, "Freshly squeezed orange juice"),
                new Product(4, "Vietnamese Coffee", 28000m, true, "Traditional ca phe sua da"),
                new Product(4, "Green Tea", 20000m, true, "Hot or iced green tea"),
                new Product(4, "Coca Cola", 18000m, true, "Chilled soft drink"),
                new Product(4, "Smoothie", 45000m, true, "Mixed fruit smoothie"),

                // Soups (CategoryId: 5)
                new Product(5, "Tom Yum Soup", 55000m, true, "Spicy Thai shrimp soup"),
                new Product(5, "Mushroom Soup", 45000m, true, "Creamy mushroom soup"),
                new Product(5, "Chicken Soup", 40000m, true, "Classic chicken noodle soup"),

                // Salads (CategoryId: 6)
                new Product(6, "Caesar Salad", 65000m, true, "Romaine lettuce with caesar dressing"),
                new Product(6, "Greek Salad", 55000m, true, "Mediterranean salad with feta"),
                new Product(6, "Garden Salad", 45000m, true, "Fresh mixed greens"),

                // Specials (CategoryId: 7)
                new Product(7, "Chef's Special", 195000m, true, "Daily special dish"),
                new Product(7, "Seafood Platter", 350000m, true, "Assorted premium seafood")
            };

            await _context.Products.AddRangeAsync(products);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Seeded {Count} products.", products.Count);
        }
    }

    /// <summary>
    /// Extension methods for DatabaseSeeder registration and usage.
    /// </summary>
    public static class DatabaseSeederExtensions
    {
        /// <summary>
        /// Seeds the database using the DatabaseSeeder service.
        /// </summary>
        public static async Task SeedDatabaseAsync(this IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
            await seeder.SeedAsync();
        }

        /// <summary>
        /// Registers the DatabaseSeeder as a scoped service.
        /// </summary>
        public static IServiceCollection AddDatabaseSeeder(this IServiceCollection services)
        {
            services.AddScoped<DatabaseSeeder>();
            return services;
        }
    }
}
