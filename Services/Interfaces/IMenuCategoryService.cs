using RestaurantManagement.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RestaurantManagement.Services.Interfaces
{
    public interface IMenuCategoryService
    {
        Task<List<MenuCategory>> GetAllCategoriesAsync();
        Task<MenuCategory> GetCategoryByIdAsync(int id);
        Task<(bool Success, string Message, MenuCategory Category)> CreateCategoryAsync(MenuCategory category);
        Task<(bool Success, string Message)> UpdateCategoryAsync(MenuCategory category);
        Task<MenuCategory> GetCategoryForDeleteAsync(int id);
        Task<(bool Success, string Message)> DeleteCategoryAsync(int id);
        Task<bool> CategoryExistsAsync(int id);
        Task<int> GetItemCountByCategoryIdAsync(int categoryId);
    }
} 