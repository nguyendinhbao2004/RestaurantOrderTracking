using Microsoft.EntityFrameworkCore;
using RestaurantOrderTracking.Domain.Entities;

namespace RestaurantOrderTracking.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // DbSet for all entities
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Area> Areas { get; set; }
        public DbSet<Bill> Bills { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<FeedBack> FeedBacks { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItems> OrderItems { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Table> Tables { get; set; }
        public DbSet<VoiceCommand> VoiceCommands { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Account -> Role (Many-to-One)
            modelBuilder.Entity<Account>(entity =>
            {
                entity.HasOne(a => a.Roles)
                    .WithMany()
                    .HasForeignKey(a => a.RoleId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // RefreshToken -> Account (Many-to-One)
            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.HasOne<Account>()
                    .WithMany(a => a.RefreshTokens)
                    .HasForeignKey(r => r.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // VoiceCommand -> Account (Many-to-One)
            modelBuilder.Entity<VoiceCommand>(entity =>
            {
                entity.HasOne(v => v.Accounts)
                    .WithMany(a => a.VoiceCommands)
                    .HasForeignKey(v => v.AccountId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(v => v.OrderItems)
                    .WithMany()
                    .HasForeignKey(v => v.OrderItemId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Table -> Area (Many-to-One)
            modelBuilder.Entity<Table>(entity =>
            {
                entity.HasOne(t => t.Area)
                    .WithMany()
                    .HasForeignKey(t => t.AreaId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Product -> Category (Many-to-One)
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasOne(p => p.Categories)
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
                    .WithMany()
                    .HasForeignKey(o => o.TableId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(o => o.Accounts)
                    .WithMany()
                    .HasForeignKey(o => o.AccountId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // OrderItems relationships
            modelBuilder.Entity<OrderItems>(entity =>
            {
                entity.HasOne(oi => oi.Order)
                    .WithMany(o => o.OrderItems)
                    .HasForeignKey(oi => oi.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(oi => oi.Products)
                    .WithMany()
                    .HasForeignKey(oi => oi.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(oi => oi.ChefAccounts)
                    .WithMany()
                    .HasForeignKey(oi => oi.ChefAccountId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(oi => oi.WaiterAccounts)
                    .WithMany()
                    .HasForeignKey(oi => oi.WaiterAccountId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Bill -> Order (One-to-One or Many-to-One)
            // Bill -> Account (Many-to-One)
            modelBuilder.Entity<Bill>(entity =>
            {
                entity.HasOne(b => b.Orders)
                    .WithMany()
                    .HasForeignKey(b => b.OrderId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(b => b.Accounts)
                    .WithMany()
                    .HasForeignKey(b => b.AccountId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.Property(b => b.Amount)
                    .HasPrecision(18, 2);
            });

            // FeedBack -> Order (Many-to-One)
            modelBuilder.Entity<FeedBack>(entity =>
            {
                entity.HasOne(f => f.Orders)
                    .WithMany()
                    .HasForeignKey(f => f.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Notification relationships
            modelBuilder.Entity<Notification>(entity =>
            {
                entity.HasOne(n => n.OrderItems)
                    .WithMany()
                    .HasForeignKey(n => n.OrderItemId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(n => n.Accounts)
                    .WithMany()
                    .HasForeignKey(n => n.AccountId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(n => n.Tables)
                    .WithMany()
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
