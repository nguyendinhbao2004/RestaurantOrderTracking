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
                _logger.LogInformation("Dữ liệu danh mục đã tồn tại, bỏ qua bước khởi tạo...");
                return;
            }

            var categories = new List<Category>
            {
                new Category(1, "Món khai vị", "Các món ăn nhẹ để bắt đầu bữa ăn"),
                new Category(2, "Món chính", "Các món ăn chính đậm đà và no bụng"),
                new Category(3, "Tráng miệng", "Món ngọt kết thúc bữa ăn tuyệt vời"),
                new Category(4, "Đồ uống", "Các loại nước giải khát tươi mát"),
                new Category(5, "Súp & Canh", "Các món súp ấm nóng và bổ dưỡng"),
                new Category(6, "Salad", "Rau trộn tươi ngon và lành mạnh"),
                new Category(7, "Món đặc biệt", "Những gợi ý đặc biệt từ đầu bếp trưởng")
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

            var products = new List<Product>
            {
                // Món khai vị (CategoryId: 1)
                new Product(1, "Chả giò rế", 45000m, true, "Chả giò rau củ chiên giòn rụm"),
                new Product(1, "Hoành thánh chiên", 38000m, true, "Hoành thánh vàng giòn kèm nước chấm đặc biệt"),
                new Product(1, "Bánh mì bơ tỏi", 25000m, true, "Bánh mì nướng bơ tỏi thơm lừng"),
                new Product(1, "Cánh gà Buffalo", 65000m, true, "Cánh gà sốt cay kiểu Buffalo"),

                // Món chính (CategoryId: 2)
                new Product(2, "Cá hồi nướng", 185000m, true, "Cá hồi Đại Tây Dương nướng thảo mộc"),
                new Product(2, "Bít tết bò", 220000m, true, "Bò thượng hạng chế biến theo yêu cầu"),
                new Product(2, "Gà sốt Parmesan", 145000m, true, "Ức gà chiên xù kèm sốt cà chua và phô mai"),
                new Product(2, "Cơm chiên rau củ", 55000m, true, "Cơm chiên tơi xốp cùng rau củ tươi"),
                new Product(2, "Pad Thai", 75000m, true, "Hủ tiếu xào kiểu Thái đặc trưng"),
                new Product(2, "Phở bò truyền thống", 65000m, true, "Phở bò với nước dùng đậm đà"),

                // Tráng miệng (CategoryId: 3)
                new Product(3, "Bánh kem Chocolate", 55000m, true, "Bánh chocolate tầng đậm đà"),
                new Product(3, "Kem Sundae", 45000m, true, "Kem vani kèm các loại topping"),
                new Product(3, "Bánh Tiramisu", 65000m, true, "Bánh hương vị cà phê kiểu Ý"),
                new Product(3, "Đĩa trái cây tươi", 75000m, true, "Trái cây tươi tổng hợp theo mùa"),

                // Đồ uống (CategoryId: 4)
                new Product(4, "Nước cam ép", 35000m, true, "Nước cam tươi nguyên chất vắt trong ngày"),
                new Product(4, "Cà phê sữa đá", 28000m, true, "Cà phê sữa đá truyền thống Việt Nam"),
                new Product(4, "Trà xanh", 20000m, true, "Trà xanh nóng hoặc đá"),
                new Product(4, "Coca Cola", 18000m, true, "Nước giải khát có gas ướp lạnh"),
                new Product(4, "Sinh tố hỗn hợp", 45000m, true, "Sinh tố trái cây tươi xay nhuyễn"),

                // Súp & Canh (CategoryId: 5)
                new Product(5, "Súp Tom Yum", 55000m, true, "Súp tôm cay nồng kiểu Thái"),
                new Product(5, "Súp nấm kem", 45000m, true, "Súp nấm kem béo ngậy"),
                new Product(5, "Súp gà ngô non", 40000m, true, "Súp gà truyền thống nấu với ngô non"),

                // Salad (CategoryId: 6)
                new Product(6, "Salad Caesar", 65000m, true, "Xà lách Romaine kèm sốt Caesar đặc trưng"),
                new Product(6, "Salad Hy Lạp", 55000m, true, "Salad Địa Trung Hải với phô mai Feta"),
                new Product(6, "Salad vườn", 45000m, true, "Các loại rau xanh hỗn hợp tươi mới"),

                // Món đặc biệt (CategoryId: 7)
                new Product(7, "Món đặc sản trong ngày", 195000m, true, "Món ăn đặc biệt do đầu bếp lựa chọn"),
                new Product(7, "Khay hải sản cao cấp", 350000m, true, "Hải sản tươi sống tổng hợp chọn lọc")
                };

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
