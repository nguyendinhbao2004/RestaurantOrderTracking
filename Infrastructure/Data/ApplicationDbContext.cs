using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
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
        public DbSet<Account> Accounts { get; set; } = null!;
        public DbSet<Area> Areas { get; set; } = null!;
        public DbSet<Bill> Bills { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<Chef> Chefs { get; set; } = null!;
        public DbSet<Customer> Customers { get; set; } = null!;
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
        public DbSet<Waiter> Waiters { get; set; } = null!;
        public DbSet<WorkSchedule> WorkSchedules { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ==================== ROLE ENTITY ====================
            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(r => r.Id);
                
                entity.Property(r => r.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(r => r.Description)
                    .HasMaxLength(500);
            });

            // ==================== ACCOUNT ENTITY ====================
            modelBuilder.Entity<Account>(entity =>
            {
                // Foreign Key and Navigation
                entity.HasOne(a => a.Role)
                    .WithMany(r => r.Accounts)
                    .HasForeignKey(a => a.RoleId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Properties
                entity.Property(a => a.UserName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(a => a.FullName)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(a => a.Phone)
                    .HasMaxLength(20);

                entity.Property(a => a.PasswordHash)
                    .IsRequired();
                entity.Property(a => a.Image);
            });

            // ==================== REFRESH TOKEN ENTITY ====================
            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.HasOne(rt => rt.User)
                    .WithMany(a => a.RefreshTokens)
                    .HasForeignKey(rt => rt.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.Property(rt => rt.Token)
                    .IsRequired();

                entity.Property(rt => rt.JwtId)
                    .IsRequired();
            });

            // ==================== WORK SCHEDULE ENTITY ====================
            modelBuilder.Entity<WorkSchedule>(entity =>
            {
                entity.HasOne(ws => ws.Account)
                    .WithMany(a => a.WorkSchedules)
                    .HasForeignKey(ws => ws.AccountId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.Property(ws => ws.ShiftName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(ws => ws.Note)
                    .HasMaxLength(500);
            });

            // ==================== CHEF ENTITY ====================
            modelBuilder.Entity<Chef>(entity =>
            {
                entity.HasOne(c => c.Account)
                    .WithMany()
                    .HasForeignKey(c => c.AccountId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.Property(c => c.Specialty)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(c => c.SkillLevel)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(c => c.Station)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasIndex(c => c.AccountId)
                    .IsUnique();
            });

            // ==================== AREA ENTITY ====================
            modelBuilder.Entity<Area>(entity =>
            {
                entity.Property(a => a.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(a => a.Description)
                    .HasMaxLength(500);
            });

            // ==================== WAITER ENTITY ====================
            modelBuilder.Entity<Waiter>(entity =>
            {
                entity.HasOne(w => w.Account)
                    .WithMany()
                    .HasForeignKey(w => w.AccountId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(w => w.AssignedArea)
                    .WithMany(a => a.Waiters)
                    .HasForeignKey(w => w.AssignedAreaId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(w => w.AccountId)
                    .IsUnique();
            });

            // ==================== TABLE ENTITY ====================
            modelBuilder.Entity<Table>(entity =>
            {
                entity.HasOne(t => t.Area)
                    .WithMany(a => a.Tables)
                    .HasForeignKey(t => t.AreaId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.Property(t => t.TableNumber)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(t => t.QRCode)
                    .HasMaxLength(500);

                entity.HasIndex(t => new { t.AreaId, t.TableNumber })
                    .IsUnique()
                    .HasName("IX_Table_AreaId_TableNumber");
            });

            // ==================== CATEGORY ENTITY ====================
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(c => c.Id);

                entity.Property(c => c.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(c => c.Description)
                    .HasMaxLength(500);

                entity.Property(c => c.ImageUrl)
                    .HasMaxLength(500);
            });

            // ==================== PRODUCT ENTITY ====================
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasOne(p => p.Category)
                    .WithMany(c => c.Products)
                    .HasForeignKey(p => p.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.Property(p => p.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(p => p.Description)
                    .HasMaxLength(1000);

                entity.Property(p => p.ImageUrl)
                    .HasMaxLength(500);

                entity.Property(p => p.Price)
                    .HasPrecision(18, 2);
            });

            // ==================== CUSTOMER ENTITY ====================
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.Property(c => c.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(c => c.Phone)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(c => c.Address)
                    .IsRequired()
                    .HasMaxLength(500);
            });

            // ==================== ORDER ENTITY ====================
            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasOne(o => o.Table)
                    .WithMany(t => t.Orders)
                    .HasForeignKey(o => o.TableId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(o => o.Customer)
                    .WithMany(c => c.Orders)
                    .HasForeignKey(o => o.CustomerId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.Property(o => o.Status)
                    .IsRequired();
            });

            // ==================== ORDER ITEM ENTITY ====================
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
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(oi => oi.WaiterAccount)
                    .WithMany()
                    .HasForeignKey(oi => oi.WaiterAccountId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.Property(oi => oi.OrderChannel)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(oi => oi.Note)
                    .HasMaxLength(500);

                entity.Property(oi => oi.UnitPrice)
                    .HasPrecision(18, 2);

                entity.Property(oi => oi.Status)
                    .IsRequired();
            });

            // ==================== ORDER ITEM LOG ENTITY ====================
            modelBuilder.Entity<OrderItemLog>(entity =>
            {
                entity.HasOne(oil => oil.OrderItem)
                    .WithMany()
                    .HasForeignKey(oil => oil.OrderItemId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(oil => oil.Account)
                    .WithMany()
                    .HasForeignKey(oil => oil.AccountId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.Property(oil => oil.Notes)
                    .HasMaxLength(500);
            });

            // ==================== BILL ENTITY ====================
            modelBuilder.Entity<Bill>(entity =>
            {
                entity.HasOne(b => b.Order)
                    .WithOne(o => o.Bill)
                    .HasForeignKey<Bill>(b => b.OrderId)
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

                entity.Property(b => b.TransactionId)
                    .HasMaxLength(100);
            });

            // ==================== FEEDBACK ENTITY ====================
            modelBuilder.Entity<FeedBack>(entity =>
            {
                entity.HasOne(f => f.Order)
                    .WithMany()
                    .HasForeignKey(f => f.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.Property(f => f.Comment)
                    .HasMaxLength(1000);
            });

            // ==================== VOICE COMMAND ENTITY ====================
            modelBuilder.Entity<VoiceCommand>(entity =>
            {
                entity.HasOne(vc => vc.Account)
                    .WithMany(a => a.VoiceCommands)
                    .HasForeignKey(vc => vc.AccountId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(vc => vc.OrderItem)
                    .WithMany()
                    .HasForeignKey(vc => vc.OrderItemId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.Property(vc => vc.AudioUrl)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(vc => vc.TranscribedText)
                    .HasMaxLength(1000);

                entity.Property(vc => vc.ParsedAction)
                    .HasMaxLength(100);

                entity.Property(vc => vc.ErrorMessage)
                    .HasMaxLength(500);
            });

            // ==================== NOTIFICATION ENTITY ====================
            modelBuilder.Entity<Notification>(entity =>
            {
                entity.HasOne(n => n.OrderItem)
                    .WithMany()
                    .HasForeignKey(n => n.OrderItemId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(n => n.Account)
                    .WithMany()
                    .HasForeignKey(n => n.AccountId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(n => n.Table)
                    .WithMany(t => t.Notifications)
                    .HasForeignKey(n => n.TableId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.Property(n => n.Title)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(n => n.Message)
                    .IsRequired()
                    .HasMaxLength(1000);
            });
        }
    }
}
