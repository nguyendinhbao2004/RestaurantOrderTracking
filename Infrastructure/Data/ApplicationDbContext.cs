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
        public DbSet<QRSession> QRSessions { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ==================== ROLE ====================
            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(r => r.Id);
                entity.Property(r => r.Name)
                    .IsRequired()
                    .HasMaxLength(100);
                entity.Property(r => r.Description)
                    .HasMaxLength(500);
            });

            // ==================== ACCOUNT ====================
            modelBuilder.Entity<Account>(entity =>
            {
                entity.HasOne(a => a.Role)
                    .WithMany(r => r.Accounts)
                    .HasForeignKey(a => a.RoleId)
                    .OnDelete(DeleteBehavior.Restrict);

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

                entity.Property(a => a.IsActive)
                    .IsRequired()
                    .HasDefaultValue(true);

                entity.Property(a => a.IsWorking)
                    .IsRequired()
                    .HasDefaultValue(false);

                entity.Property(a => a.Image)
                    .HasMaxLength(500);
            });

            // ==================== AREA ====================
            modelBuilder.Entity<Area>(entity =>
            {
                entity.Property(a => a.Name)
                    .IsRequired()
                    .HasMaxLength(100);
                entity.Property(a => a.Description)
                    .HasMaxLength(500);
            });

            // ==================== WAITER ====================
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

                entity.Property(w => w.OrderCount)
                    .IsRequired();

                entity.HasIndex(w => w.AccountId)
                    .IsUnique();
            });

            // ==================== TABLE ====================
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

                entity.Property(t => t.Capacity)
                    .IsRequired();

                entity.HasIndex(t => new { t.AreaId, t.TableNumber })
                    .IsUnique()
                    .HasName("IX_Table_AreaId_TableNumber");
            });

            // ==================== CATEGORY ====================
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

                entity.Property(c => c.IsActive)
                    .IsRequired()
                    .HasDefaultValue(true);
            });

            // ==================== PRODUCT ====================
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

                entity.Property(p => p.IsActive)
                    .IsRequired()
                    .HasDefaultValue(true);
            });

            // ==================== CUSTOMER ====================
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

            // ==================== ORDER ====================
            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasOne(o => o.Table)
                    .WithMany(t => t.Orders)
                    .HasForeignKey(o => o.TableId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(o => o.Waiter)
                  .WithMany()
                  .HasForeignKey(o => o.WaiterId)
                  .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(o => o.Customer)
                    .WithMany(c => c.Orders)
                    .HasForeignKey(o => o.CustomerId)
                    .OnDelete(DeleteBehavior.SetNull);
                entity.Property(o => o.OrderTypes)
                    .IsRequired();

                entity.Property(o => o.Status)
                    .IsRequired();
            });

            // ==================== ORDER ITEM ====================
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


                entity.Property(oi => oi.Status)
                    .IsRequired();
            });

            // ==================== ORDER ITEM LOG ====================
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

                entity.Property(oil => oil.PreviousStatus)
                    .IsRequired();

                entity.Property(oil => oil.NewStatus)
                    .IsRequired();

                entity.Property(oil => oil.Notes)
                    .HasMaxLength(500);
            });

            // ==================== BILL ====================
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

                entity.Property(b => b.Tax)
                    .HasPrecision(5, 2);

                entity.Property(b => b.PaymentMethod)
                    .IsRequired();

                entity.Property(b => b.Status)
                    .IsRequired();

                entity.Property(b => b.PaidAt);

                entity.Property(b => b.TransactionId)
                    .HasMaxLength(100);
            });

            // ==================== FEEDBACK ====================
            modelBuilder.Entity<FeedBack>(entity =>
            {
                entity.HasOne(f => f.Order)
                    .WithMany()
                    .HasForeignKey(f => f.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.Property(f => f.Rating)
                    .IsRequired();

                entity.Property(f => f.Comment)
                    .HasMaxLength(1000);

                entity.Property(f => f.IsAnonymous)
                    .IsRequired()
                    .HasDefaultValue(false);
            });

            // ==================== VOICE COMMAND ====================
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

                entity.Property(vc => vc.ParsedTableId);

                entity.Property(vc => vc.ParsedProductName)
                    .HasMaxLength(200);

                entity.Property(vc => vc.ConfidenceScore);

                entity.Property(vc => vc.Status)
                    .IsRequired();

                entity.Property(vc => vc.ProcessedAt);

                entity.Property(vc => vc.ErrorMessage)
                    .HasMaxLength(500);
            });

            // ==================== NOTIFICATION ====================
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

                entity.Property(n => n.Type)
                    .IsRequired();

                entity.Property(n => n.Message)
                    .IsRequired()
                    .HasMaxLength(1000);

                entity.Property(n => n.IsRead)
                    .IsRequired()
                    .HasDefaultValue(false);

                entity.Property(n => n.ReadAt);
            });

            // ==================== REFRESH TOKEN ====================
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

                entity.Property(rt => rt.IsUsed)
                    .IsRequired()
                    .HasDefaultValue(false);

                entity.Property(rt => rt.IsRevoked)
                    .IsRequired()
                    .HasDefaultValue(false);

                entity.Property(rt => rt.Expires)
                    .IsRequired();

                entity.Property(rt => rt.AddedDate)
                    .IsRequired();
            });

            // ==================== WORK SCHEDULE ====================
            modelBuilder.Entity<WorkSchedule>(entity =>
            {
                entity.HasOne(ws => ws.Account)
                    .WithMany(a => a.WorkSchedules)
                    .HasForeignKey(ws => ws.AccountId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.Property(ws => ws.WorkDate)
                    .IsRequired();

                entity.Property(ws => ws.StartTime)
                    .IsRequired();

                entity.Property(ws => ws.EndTime)
                    .IsRequired();

                entity.Property(ws => ws.ShiftName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(ws => ws.ActualCheckIn);

                entity.Property(ws => ws.ActualCheckOut);

                entity.Property(ws => ws.Status)
                    .IsRequired();

                entity.Property(ws => ws.Note)
                    .HasMaxLength(500);
            });

            // ==================== QR SESSION ====================
            modelBuilder.Entity<QRSession>(entity =>
            {
                entity.HasOne(qs => qs.Table)
                    .WithMany()
                    .HasForeignKey(qs => qs.TableId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.Property(qs => qs.SessionToken)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(qs => qs.ExpiresAt)
                    .IsRequired();

                entity.Property(qs => qs.IsActive)
                    .IsRequired()
                    .HasDefaultValue(true);

                entity.HasIndex(qs => qs.SessionToken)
                    .IsUnique()
                    .HasDatabaseName("IX_QRSession_SessionToken");

                entity.HasIndex(qs => new { qs.TableId, qs.IsActive })
                    .HasDatabaseName("IX_QRSession_TableId_IsActive");
            });
        }
    }
}