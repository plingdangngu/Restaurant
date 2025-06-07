using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RestaurantManagement.Models;

namespace RestaurantManagement.Data
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>());

            // Check if DB has been seeded
            if (context.MenuCategories.Any() || context.MenuItems.Any() || 
                context.Floors.Any() || context.Tables.Any())
            {
                return;
            }

            // Seed MenuCategories
            var categories = new MenuCategory[]
            {
                new MenuCategory { Name = "Món khai vị", Description = "Các món ăn khai vị", IsAvailable = true },
                new MenuCategory { Name = "Món chính", Description = "Các món ăn chính", IsAvailable = true },
                new MenuCategory { Name = "Món tráng miệng", Description = "Các món tráng miệng", IsAvailable = true },
                new MenuCategory { Name = "Đồ uống", Description = "Các loại đồ uống", IsAvailable = true },
            };
            context.MenuCategories.AddRange(categories);
            context.SaveChanges();

            // Seed MenuItems
            var menuItems = new MenuItem[]
            {
                new MenuItem { Name = "Gỏi cuốn", Price = 35000, MenuCategoryId = categories[0].Id, Description = "Gỏi cuốn tôm thịt", IsAvailable = true },
                new MenuItem { Name = "Chả giò", Price = 40000, MenuCategoryId = categories[0].Id, Description = "Chả giò giòn rụm", IsAvailable = true },
                new MenuItem { Name = "Phở bò", Price = 65000, MenuCategoryId = categories[1].Id, Description = "Phở bò truyền thống", IsAvailable = true },
                new MenuItem { Name = "Cơm gà", Price = 55000, MenuCategoryId = categories[1].Id, Description = "Cơm gà xối mỡ", IsAvailable = true },
                new MenuItem { Name = "Chè khúc bạch", Price = 25000, MenuCategoryId = categories[2].Id, Description = "Chè khúc bạch mát lạnh", IsAvailable = true },
                new MenuItem { Name = "Trà sữa", Price = 30000, MenuCategoryId = categories[3].Id, Description = "Trà sữa trân châu", IsAvailable = true },
                new MenuItem { Name = "Cafe đen", Price = 25000, MenuCategoryId = categories[3].Id, Description = "Cafe đen đá", IsAvailable = true },
            };
            context.MenuItems.AddRange(menuItems);
            context.SaveChanges();

            // Seed Floors
            var floors = new Floor[]
            {
                new Floor { Name = "Tầng 1", Description = "Tầng trệt" },
                new Floor { Name = "Tầng 2", Description = "Tầng lầu" },
            };
            context.Floors.AddRange(floors);
            context.SaveChanges();

            // Seed Tables
            var tables = new Table[]
            {
                new Table { Name = "Bàn 101", Capacity = 4, Status = "Available", FloorId = floors[0].Id },
                new Table { Name = "Bàn 102", Capacity = 4, Status = "Available", FloorId = floors[0].Id },
                new Table { Name = "Bàn 103", Capacity = 6, Status = "Available", FloorId = floors[0].Id },
                new Table { Name = "Bàn 104", Capacity = 8, Status = "Available", FloorId = floors[0].Id },
                new Table { Name = "Bàn 201", Capacity = 4, Status = "Available", FloorId = floors[1].Id },
                new Table { Name = "Bàn 202", Capacity = 4, Status = "Available", FloorId = floors[1].Id },
                new Table { Name = "Bàn 203", Capacity = 6, Status = "Available", FloorId = floors[1].Id },
                new Table { Name = "Bàn 204", Capacity = 8, Status = "Available", FloorId = floors[1].Id },
            };
            context.Tables.AddRange(tables);
            context.SaveChanges();
        }
    }
}
