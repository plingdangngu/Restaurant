using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Data;
using RestaurantManagement.Models;
using RestaurantManagement.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantManagement.Services.Implementations
{
    public class MenuCategoryService : IMenuCategoryService
    {
        private readonly ApplicationDbContext _context;

        public MenuCategoryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<MenuCategory>> GetAllCategoriesAsync()
        {
            return await _context.MenuCategories
                .OrderBy(c => c.DisplayOrder)
                .ToListAsync();
        }

        public async Task<MenuCategory> GetCategoryByIdAsync(int id)
        {
            return await _context.MenuCategories
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<(bool Success, string Message, MenuCategory Category)> CreateCategoryAsync(MenuCategory category)
        {
            if (category == null)
                return (false, "Danh mục không hợp lệ", null);

            try
            {
                _context.Add(category);
                await _context.SaveChangesAsync();
                return (true, "Thêm danh mục thành công!", category);
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi: {ex.Message}", null);
            }
        }

        public async Task<(bool Success, string Message)> UpdateCategoryAsync(MenuCategory category)
        {
            if (category == null)
                return (false, "Danh mục không hợp lệ");

            try
            {
                _context.Update(category);
                await _context.SaveChangesAsync();
                return (true, "Cập nhật danh mục thành công!");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await CategoryExistsAsync(category.Id))
                    return (false, "Không tìm thấy danh mục");
                else
                    throw;
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi: {ex.Message}");
            }
        }

        public async Task<MenuCategory> GetCategoryForDeleteAsync(int id)
        {
            return await _context.MenuCategories
                .Include(c => c.MenuItems)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<(bool Success, string Message)> DeleteCategoryAsync(int id)
        {
            try
            {
                // Kiểm tra xem danh mục có chứa món ăn không
                var itemCount = await GetItemCountByCategoryIdAsync(id);
                if (itemCount > 0)
                    return (false, $"Không thể xóa danh mục này vì có {itemCount} món ăn thuộc danh mục");

                var category = await _context.MenuCategories.FindAsync(id);
                if (category == null)
                    return (false, "Không tìm thấy danh mục");

                _context.MenuCategories.Remove(category);
                await _context.SaveChangesAsync();
                return (true, "Xóa danh mục thành công!");
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi: {ex.Message}");
            }
        }

        public async Task<bool> CategoryExistsAsync(int id)
        {
            return await _context.MenuCategories.AnyAsync(c => c.Id == id);
        }

        public async Task<int> GetItemCountByCategoryIdAsync(int categoryId)
        {
            return await _context.MenuItems
                .CountAsync(m => m.CategoryId == categoryId);
        }
    }
} 