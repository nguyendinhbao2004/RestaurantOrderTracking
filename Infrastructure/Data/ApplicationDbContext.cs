using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RestaurantOrderTracking.Domain.Entities;

namespace RestaurantOrderTracking.Infrastructure.Data
{
    // FIX: Inherit only from IdentityDbContext<Account> (which itself inherits from DbContext)
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // DbSet for all entities
        public DbSet<Account> Accounts { get; set; } = null!;
        public DbSet<Area> Areas { get; set; } = null!;
        public DbSet<Bill> Bills { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<FeedBack> FeedBacks { get; set; } = null!;
        public DbSet<Notification> Notifications { get; set; } = null!;
        public DbSet<Order> Orders { get; set; } = null!;
        public DbSet<OrderItem> OrderItems { get; set; } = null!;
        public DbSet<OrderItemLog> OrderItemLogs { get; set; } = null!;
        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;
        public DbSet<Role> Roles { get; set; } = null!;
        public DbSet<Table> Tables { get; set; } = null!;
        public DbSet<VoiceCommand> VoiceCommands { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Account -> Role (Many-to-One)
            modelBuilder.Entity<Account>(entity =>
            {
                entity.HasOne(a => a.Role)
                    .WithMany(r => r.Accounts)
                    .HasForeignKey(a => a.RoleId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // RefreshToken -> Account (Many-to-One)
            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.HasOne(r => r.User)
                    .WithMany(a => a.RefreshTokens)
                    .HasForeignKey(r => r.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // VoiceCommand -> Account (Many-to-One)
            modelBuilder.Entity<VoiceCommand>(entity =>
            {
                entity.HasOne(v => v.Account)
                    .WithMany(a => a.VoiceCommands)
                    .HasForeignKey(v => v.AccountId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(v => v.OrderItem)
                    .WithMany()
                    .HasForeignKey(v => v.OrderItemId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Table -> Area (Many-to-One)
            modelBuilder.Entity<Table>(entity =>
            {
                entity.HasOne(t => t.Area)
                    .WithMany(a => a.Tables)
                    .HasForeignKey(t => t.AreaId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Product -> Category (Many-to-One)
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasOne(p => p.Category)
                    .WithMany(c => c.Products)
                    .HasForeignKey(p => p.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.Property(p => p.Price)
                    .HasPrecision(18, 2);
            });

            // Order -> Table (Many-to-One)
            // Order -> Account (Many-to-One)
            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasOne(o => o.Table)
                    .WithMany(t => t.Orders)
                    .HasForeignKey(o => o.TableId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(o => o.Account)
                    .WithMany()
                    .HasForeignKey(o => o.AccountId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // OrderItem relationships
            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.HasOne(oi => oi.Order)
                    .WithMany(o => o.OrderItems)
                    .HasForeignKey(oi => oi.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(oi => oi.Product)
                    .WithMany(p => p.OrderItems)
                    .HasForeignKey(oi => oi.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(oi => oi.ChefAccount)
                    .WithMany()
                    .HasForeignKey(oi => oi.ChefAccountId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(oi => oi.WaiterAccount)
                    .WithMany()
                    .HasForeignKey(oi => oi.WaiterAccountId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.Property(oi => oi.UnitPrice)
                    .HasPrecision(18, 2);
            });

            // OrderItemLog relationships
            modelBuilder.Entity<OrderItemLog>(entity =>
            {
                entity.HasOne(oil => oil.OrderItem)
                    .WithMany()
                    .HasForeignKey(oil => oil.OrderItemId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(oil => oil.Account)
                    .WithMany()
                    .HasForeignKey(oil => oil.AccountId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Bill -> Order (Many-to-One)
            // Bill -> Account (Many-to-One)
            modelBuilder.Entity<Bill>(entity =>
            {
                entity.HasOne(b => b.Order)
                    .WithMany()
                    .HasForeignKey(b => b.OrderId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(b => b.Account)
                    .WithMany(a => a.Bills)
                    .HasForeignKey(b => b.AccountId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.Property(b => b.Amount)
                    .HasPrecision(18, 2);

                entity.Property(b => b.Discount)
                    .HasPrecision(18, 2);

                entity.Property(b => b.FinalAmount)
                    .HasPrecision(18, 2);
            });

            // FeedBack -> Order (Many-to-One)
            modelBuilder.Entity<FeedBack>(entity =>
            {
                entity.HasOne(f => f.Order)
                    .WithMany()
                    .HasForeignKey(f => f.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Notification relationships
            modelBuilder.Entity<Notification>(entity =>
            {
                entity.HasOne(n => n.OrderItem)
                    .WithMany()
                    .HasForeignKey(n => n.OrderItemId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(n => n.Account)
                    .WithMany()
                    .HasForeignKey(n => n.AccountId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(n => n.Table)
                    .WithMany(t => t.Notifications)
                    .HasForeignKey(n => n.TableId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Category configuration
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(c => c.Id);
            });

            // Role configuration
            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(r => r.Id);
            });
        }
    }
}
