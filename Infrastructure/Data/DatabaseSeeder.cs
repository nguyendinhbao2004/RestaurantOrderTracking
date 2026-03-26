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
                new Role("Admin", "System administrator with full access"),
                new Role("Manager", "Restaurant manager with management access"),
                new Role("Chef", "Kitchen staff responsible for cooking"),
                new Role("Waiter", "Service staff responsible for serving customers"),
                new Role("Cashier", "Staff responsible for handling payments"),
                new Role("HeadChef", "Head chef responsible for menu creation and kitchen management")
            };

            await _context.Roles.AddRangeAsync(roles);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Seeded {Count} roles.", roles.Count);
        }

        private async Task SeedCategoriesAsync()
        {
            if (await _context.Categories.AnyAsync())
            {
                _logger.LogInformation("Dữ liệu danh mục đã tồn tại, bỏ qua bước khởi tạo...");
                return;
            }

            var categories = new List<Category>
            {
                new Category("Món Á", "Các món ăn Châu Á"),
                new Category("Món Âu", "Các món ăn Châu Á"),
                new Category("Đồ uống Pha Chế", "Món nước pha chế"),
                new Category("Nước Ngọt", "Nước ngọt"),
            };
            await _context.Categories.AddRangeAsync(categories);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Đã khởi tạo thành công {Count} danh mục.", categories.Count);
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
                _logger.LogInformation("Dữ liệu sản phẩm đã tồn tại, bỏ qua bước khởi tạo...");
                return;
            }

            // Lấy danh sách category từ database để lấy đúng Id
            var categories = await _context.Categories.ToListAsync();
            var categoryMonA = categories.FirstOrDefault(c => c.Name == "Món Á");
            var categoryMonAu = categories.FirstOrDefault(c => c.Name == "Món Âu");
            var categoryDoUongPhaChe = categories.FirstOrDefault(c => c.Name == "Đồ uống Pha Chế");
            var categoryNuocNgot = categories.FirstOrDefault(c => c.Name == "Nước Ngọt");

            var products = new List<Product>();

            if (categoryMonA != null)
            {
                products.AddRange(new[]
                {
                    new Product(categoryMonA.Id, "Phở bò", 65000m, true, "Phở bò truyền thống Việt Nam"),
                    new Product(categoryMonA.Id, "Cơm chiên Dương Châu", 55000m, true, "Cơm chiên kiểu Dương Châu"),
                    new Product(categoryMonA.Id, "Gỏi cuốn tôm thịt", 40000m, true, "Gỏi cuốn tươi ngon"),
                    new Product(categoryMonA.Id, "Bún chả Hà Nội", 70000m, true, "Bún chả đặc sản Hà Nội")
                });
            }
            if (categoryMonAu != null)
            {
                products.AddRange(new[]
                {
                    new Product(categoryMonAu.Id, "Bít tết bò", 220000m, true, "Bò thượng hạng chế biến theo yêu cầu"),
                    new Product(categoryMonAu.Id, "Cá hồi nướng", 185000m, true, "Cá hồi Đại Tây Dương nướng thảo mộc"),
                    new Product(categoryMonAu.Id, "Mỳ Ý sốt bò bằm", 95000m, true, "Mỳ Ý truyền thống với sốt bò bằm"),
                    new Product(categoryMonAu.Id, "Salad Caesar", 65000m, true, "Xà lách Romaine kèm sốt Caesar đặc trưng")
                });
            }
            if (categoryDoUongPhaChe != null)
            {
                products.AddRange(new[]
                {
                    new Product(categoryDoUongPhaChe.Id, "Trà đào cam sả", 35000m, true, "Trà đào cam sả mát lạnh"),
                    new Product(categoryDoUongPhaChe.Id, "Sinh tố bơ", 40000m, true, "Sinh tố bơ tươi"),
                    new Product(categoryDoUongPhaChe.Id, "Nước ép dưa hấu", 30000m, true, "Nước ép dưa hấu nguyên chất"),
                    new Product(categoryDoUongPhaChe.Id, "Cà phê sữa đá", 28000m, true, "Cà phê sữa đá truyền thống Việt Nam")
                });
            }
            if (categoryNuocNgot != null)
            {
                products.AddRange(new[]
                {
                    new Product(categoryNuocNgot.Id, "Coca Cola", 18000m, true, "Nước giải khát có gas ướp lạnh"),
                    new Product(categoryNuocNgot.Id, "Pepsi", 18000m, true, "Nước giải khát có gas Pepsi"),
                    new Product(categoryNuocNgot.Id, "7Up", 18000m, true, "Nước giải khát có gas 7Up"),
                    new Product(categoryNuocNgot.Id, "Nước suối", 12000m, true, "Nước suối đóng chai")
                });
            }

            await _context.Products.AddRangeAsync(products);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Đã khởi tạo thành công {Count} sản phẩm.", products.Count);
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
