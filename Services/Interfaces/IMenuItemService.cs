using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using RestaurantManagement.Models;
using RestaurantManagement.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RestaurantManagement.Services.Interfaces
{
    public interface IMenuItemService
    {
        Task<List<MenuItem>> GetAllMenuItemsAsync();
        Task<MenuItem> GetMenuItemByIdAsync(int id);
        Task<MenuItemViewModel> PrepareCreateViewModelAsync();
        Task<(bool Success, string Message, MenuItem MenuItem)> CreateMenuItemAsync(MenuItemViewModel model, IFormFile imageFile);
        Task<MenuItemViewModel> PrepareEditViewModelAsync(int id);
        Task<(bool Success, string Message)> UpdateMenuItemAsync(MenuItemViewModel model, IFormFile imageFile);
        Task<MenuItem> GetMenuItemForDeleteAsync(int id);
        Task<(bool Success, string Message)> DeleteMenuItemAsync(int id);
        Task<bool> MenuItemExistsAsync(int id);
        Task<Dictionary<string, List<MenuItem>>> GetMenuItemsGroupedByCategoryAsync();
        Task<List<MenuItem>> GetMenuItemsByCategoryIdAsync(int categoryId);
        Task<SelectList> GetMenuCategoriesSelectListAsync(int? selectedCategoryId = null);
        Task<CustomerMenuViewModel> PrepareCustomerMenuViewModelAsync(int? categoryId);
        Task<(bool Success, string Message)> ToggleMenuItemStatusAsync(int id);
        Task<(bool Success, string Message)> ToggleMenuItemFeaturedAsync(int id);
    }
} 