using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Models;


namespace RestaurantManagement.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        
        public DbSet<Floor> Floors { get; set; }
        public DbSet<Table> Tables { get; set; }
        public DbSet<MenuCategory> MenuCategories { get; set; }
        public DbSet<MenuItem> MenuItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            
            // Define table relationships
            
            // Floor - Table (1:N)
            builder.Entity<Table>()
                .HasOne(t => t.Floor)
                .WithMany(f => f.Tables)
                .HasForeignKey(t => t.FloorId)
                .OnDelete(DeleteBehavior.Restrict);
                
            // Table - Order (1:N)
            builder.Entity<Order>()
                .HasOne(o => o.Table)
                .WithMany(t => t.Orders)
                .HasForeignKey(o => o.TableId)
                .OnDelete(DeleteBehavior.Restrict);
                
            // MenuCategory - MenuItem (1:N)
            builder.Entity<MenuItem>()
                .HasOne(m => m.MenuCategory)
                .WithMany(c => c.MenuItems)
                .HasForeignKey(m => m.MenuCategoryId)
                .OnDelete(DeleteBehavior.Restrict);
                
            // MenuItem - OrderItem (1:N)
            builder.Entity<OrderItem>()
                .HasOne(oi => oi.MenuItem)
                .WithMany(m => m.OrderItems)
                .HasForeignKey(oi => oi.MenuItemId)
                .OnDelete(DeleteBehavior.Restrict);
                
            // Order - OrderItem (1:N)
            builder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
                
            // Customer - Order (1:N)
            builder.Entity<Order>()
                .HasOne(o => o.Customer)
                .WithMany(c => c.Orders)
                .HasForeignKey(o => o.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);
                
            // ApplicationUser (Employee) - Order (1:N)
            builder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany(e => e.Orders)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Restrict);
                
            // Order - Invoice (1:1)
            builder.Entity<Invoice>()
                .HasOne(i => i.Order)
                .WithOne(o => o.Invoice)
                .HasForeignKey<Invoice>(i => i.OrderId)
                .OnDelete(DeleteBehavior.Restrict);
                
            // Customer - Reservation (1:N)
            builder.Entity<Reservation>()
                .HasOne(r => r.Customer)
                .WithMany()
                .HasForeignKey(r => r.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);
                
            // Table - Reservation (1:N)
            builder.Entity<Reservation>()
                .HasOne(r => r.Table)
                .WithMany()
                .HasForeignKey(r => r.TableId)
                .OnDelete(DeleteBehavior.Restrict);

            // Seed data

        }
    }
} 