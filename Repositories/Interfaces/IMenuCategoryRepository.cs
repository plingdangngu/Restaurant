using RestaurantManagement.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RestaurantManagement.Repositories.Interfaces
{
    public interface IMenuCategoryRepository
    {
        Task<List<MenuCategory>> GetAllAsync();
        Task<MenuCategory> GetByIdAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<bool> NameExistsAsync(string name, int? excludeId = null);
        Task AddAsync(MenuCategory category);
        void Update(MenuCategory category);
        void Remove(MenuCategory category);
        Task<bool> HasMenuItemsAsync(int id);
        Task SaveChangesAsync();
    }
} 