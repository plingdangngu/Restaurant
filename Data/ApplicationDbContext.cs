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
            // Seed MenuCategories
            var categories = new[]
            {
                new MenuCategory { Id = 1, Name = "Món khai vị", Description = "Các món ăn khai vị", IsAvailable = true },
                new MenuCategory { Id = 2, Name = "Món chính", Description = "Các món ăn chính", IsAvailable = true },
                new MenuCategory { Id = 3, Name = "Món tráng miệng", Description = "Các món tráng miệng", IsAvailable = true },
                new MenuCategory { Id = 4, Name = "Đồ uống", Description = "Các loại đồ uống", IsAvailable = true },
            };
            builder.Entity<MenuCategory>().HasData(categories);

            // Seed MenuItems
            var menuItems = new[]
            {
                new MenuItem { Id = 1, Name = "Gỏi cuốn", Price = 35000, MenuCategoryId = 1, Description = "Gỏi cuốn tôm thịt", IsAvailable = true },
                new MenuItem { Id = 2, Name = "Chả giò", Price = 40000, MenuCategoryId = 1, Description = "Chả giò giòn rụm", IsAvailable = true },
                new MenuItem { Id = 3, Name = "Phở bò", Price = 65000, MenuCategoryId = 2, Description = "Phở bò truyền thống", IsAvailable = true },
                new MenuItem { Id = 4, Name = "Cơm gà", Price = 55000, MenuCategoryId = 2, Description = "Cơm gà xối mỡ", IsAvailable = true },
                new MenuItem { Id = 5, Name = "Chè khúc bạch", Price = 25000, MenuCategoryId = 3, Description = "Chè khúc bạch mát lạnh", IsAvailable = true },
                new MenuItem { Id = 6, Name = "Trà sữa", Price = 30000, MenuCategoryId = 4, Description = "Trà sữa trân châu", IsAvailable = true },
                new MenuItem { Id = 7, Name = "Cafe đen", Price = 25000, MenuCategoryId = 4, Description = "Cafe đen đá", IsAvailable = true },
            };
            builder.Entity<MenuItem>().HasData(menuItems);

            // Seed Floors
            var floors = new[]
            {
                new Floor { Id = 1, Name = "Tầng 1", Description = "Tầng trệt" },
                new Floor { Id = 2, Name = "Tầng 2", Description = "Tầng lầu" },
            };
            builder.Entity<Floor>().HasData(floors);

            // Seed Tables
            var tables = new[]
            {
                new Table { Id = 1, Name = "Bàn 101", Capacity = 4, Status = "Available", FloorId = 1 },
                new Table { Id = 2, Name = "Bàn 102", Capacity = 4, Status = "Available", FloorId = 1 },
                new Table { Id = 3, Name = "Bàn 103", Capacity = 6, Status = "Available", FloorId = 1 },
                new Table { Id = 4, Name = "Bàn 104", Capacity = 8, Status = "Available", FloorId = 1 },
                new Table { Id = 5, Name = "Bàn 201", Capacity = 4, Status = "Available", FloorId = 2 },
                new Table { Id = 6, Name = "Bàn 202", Capacity = 4, Status = "Available", FloorId = 2 },
                new Table { Id = 7, Name = "Bàn 203", Capacity = 6, Status = "Available", FloorId = 2 },
                new Table { Id = 8, Name = "Bàn 204", Capacity = 8, Status = "Available", FloorId = 2 },
            };
            builder.Entity<Table>().HasData(tables);
        }
    }
} 