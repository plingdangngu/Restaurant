using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Data;
using RestaurantManagement.Models;
using RestaurantManagement.Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantManagement.Repositories.Implementations
{
    public class MenuCategoryRepository : IMenuCategoryRepository
    {
        private readonly ApplicationDbContext _context;

        public MenuCategoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<MenuCategory>> GetAllAsync()
        {
            return await _context.MenuCategories
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<MenuCategory> GetByIdAsync(int id)
        {
            return await _context.MenuCategories.FindAsync(id);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.MenuCategories.AnyAsync(c => c.Id == id);
        }

        public async Task<bool> NameExistsAsync(string name, int? excludeId = null)
        {
            if (excludeId.HasValue)
                return await _context.MenuCategories
                    .AnyAsync(c => c.Name == name && c.Id != excludeId.Value);
            else
                return await _context.MenuCategories
                    .AnyAsync(c => c.Name == name);
        }

        public async Task AddAsync(MenuCategory category)
        {
            await _context.MenuCategories.AddAsync(category);
        }

        public void Update(MenuCategory category)
        {
            _context.MenuCategories.Update(category);
        }

        public void Remove(MenuCategory category)
        {
            _context.MenuCategories.Remove(category);
        }

        public async Task<bool> HasMenuItemsAsync(int id)
        {
            return await _context.MenuItems
                .AnyAsync(m => m.CategoryId == id);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
} 