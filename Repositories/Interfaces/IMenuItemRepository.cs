using RestaurantManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace RestaurantManagement.Repositories.Interfaces
{
    public interface IMenuItemRepository
    {
        Task<List<MenuItem>> GetAllAsync();
        Task<IQueryable<MenuItem>> GetAllAsync(Expression<Func<MenuItem, bool>> predicate);
        Task<IQueryable<T>> GetAllAsync<T>(Expression<Func<T, bool>> predicate) where T : class;
        Task<MenuItem> GetByIdAsync(int id);
        Task<MenuItem> GetByIdWithCategoryAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<bool> NameExistsAsync(string name, int? excludeId = null);
        Task AddAsync(MenuItem menuItem);
        void Update(MenuItem menuItem);
        void Remove(MenuItem menuItem);
        Task<List<MenuItem>> GetByCategoryIdAsync(int categoryId);
        Task<List<MenuItem>> GetActiveMenuItemsAsync();
        Task<List<MenuItem>> SearchMenuItemsAsync(string searchTerm);
        Task<List<MenuCategory>> GetCategoriesAsync();
        Task SaveChangesAsync();
    }
} 