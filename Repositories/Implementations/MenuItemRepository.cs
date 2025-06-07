using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Data;
using RestaurantManagement.Models;
using RestaurantManagement.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace RestaurantManagement.Repositories.Implementations
{
    public class MenuItemRepository : IMenuItemRepository
    {
        private readonly ApplicationDbContext _context;

        public MenuItemRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<MenuItem>> GetAllAsync()
        {
            return await _context.MenuItems
                .Include(m => m.Category)
                .OrderBy(m => m.Category.Name)
                .ThenBy(m => m.Name)
                .ToListAsync();
        }

        public Task<IQueryable<MenuItem>> GetAllAsync(Expression<Func<MenuItem, bool>> predicate)
        {
            return Task.FromResult(_context.MenuItems
                .Include(m => m.Category)
                .Where(predicate));
        }

        public Task<IQueryable<T>> GetAllAsync<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            return Task.FromResult(_context.Set<T>().Where(predicate));
        }

        public async Task<MenuItem> GetByIdAsync(int id)
        {
            return await _context.MenuItems.FindAsync(id);
        }

        public async Task<MenuItem> GetByIdWithCategoryAsync(int id)
        {
            return await _context.MenuItems
                .Include(m => m.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.MenuItems.AnyAsync(m => m.Id == id);
        }

        public async Task<bool> NameExistsAsync(string name, int? excludeId = null)
        {
            if (excludeId.HasValue)
                return await _context.MenuItems
                    .AnyAsync(m => m.Name == name && m.Id != excludeId.Value);
            else
                return await _context.MenuItems.AnyAsync(m => m.Name == name);
        }

        public async Task AddAsync(MenuItem menuItem)
        {
            await _context.MenuItems.AddAsync(menuItem);
        }

        public void Update(MenuItem menuItem)
        {
            _context.MenuItems.Update(menuItem);
        }

        public void Remove(MenuItem menuItem)
        {
            _context.MenuItems.Remove(menuItem);
        }

        public async Task<List<MenuItem>> GetByCategoryIdAsync(int categoryId)
        {
            return await _context.MenuItems
                .Where(m => m.CategoryId == categoryId)
                .OrderBy(m => m.Name)
                .ToListAsync();
        }

        public async Task<List<MenuItem>> GetActiveMenuItemsAsync()
        {
            return await _context.MenuItems
                .Include(m => m.Category)
                .Where(m => m.IsAvailable)
                .OrderBy(m => m.Category.Name)
                .ThenBy(m => m.Name)
                .ToListAsync();
        }

        public async Task<List<MenuItem>> SearchMenuItemsAsync(string searchTerm)
        {
            return await _context.MenuItems
                .Include(m => m.Category)
                .Where(m => m.Name.Contains(searchTerm) || 
                       m.Description.Contains(searchTerm) || 
                       m.Category.Name.Contains(searchTerm))
                .OrderBy(m => m.Category.Name)
                .ThenBy(m => m.Name)
                .ToListAsync();
        }

        public async Task<List<MenuCategory>> GetCategoriesAsync()
        {
            return await _context.MenuCategories
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
} 