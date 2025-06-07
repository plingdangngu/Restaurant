using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Data;
using RestaurantManagement.Models;
using RestaurantManagement.Services.Interfaces;
using RestaurantManagement.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantManagement.Services.Implementations
{
    public class MenuItemService : IMenuItemService
    {
        private readonly ApplicationDbContext _context;
        private readonly string _uploadsFolder;

        public MenuItemService(ApplicationDbContext context)
        {
            _context = context;
            _uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
        }

        public async Task<List<MenuItem>> GetAllMenuItemsAsync()
        {
            return await _context.MenuItems
                .Include(m => m.Category)
                .OrderBy(m => m.Category.DisplayOrder)
                .ThenBy(m => m.DisplayOrder)
                .ToListAsync();
        }

        public async Task<MenuItem> GetMenuItemByIdAsync(int id)
        {
            return await _context.MenuItems
                .Include(m => m.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<MenuItemViewModel> PrepareCreateViewModelAsync()
        {
            var viewModel = new MenuItemViewModel
            {
                IsAvailable = true,
                MenuCategories = await GetMenuCategoriesSelectListAsync()
            };
            
            return viewModel;
        }

        public async Task<(bool Success, string Message, MenuItem MenuItem)> CreateMenuItemAsync(MenuItemViewModel model, IFormFile imageFile)
        {
            if (model == null)
                return (false, "Model không hợp lệ", null);

            try
            {
                // Kiểm tra danh mục tồn tại
                var category = await _context.MenuCategories.FindAsync(model.CategoryId);
                if (category == null)
                    return (false, "Danh mục không tồn tại", null);

                var menuItem = new MenuItem
                {
                    Name = model.Name,
                    Description = model.Description,
                    Price = model.Price,
                    DiscountedPrice = model.DiscountedPrice,
                    CategoryId = model.CategoryId,
                    IsAvailable = model.IsAvailable,
                    DisplayOrder = model.DisplayOrder
                };

                // Xử lý tải lên hình ảnh
                if (imageFile != null && imageFile.Length > 0)
                {
                    // Đảm bảo thư mục tồn tại
                    Directory.CreateDirectory(_uploadsFolder);
                    
                    // Tạo tên file duy nhất
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                    var filePath = Path.Combine(_uploadsFolder, fileName);
                    
                    // Lưu file
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                    }
                    
                    // Cập nhật đường dẫn hình ảnh
                    menuItem.ImageUrl = "/uploads/" + fileName;
                }

                _context.Add(menuItem);
                await _context.SaveChangesAsync();

                return (true, "Thêm món ăn thành công!", menuItem);
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi: {ex.Message}", null);
            }
        }

        public async Task<MenuItemViewModel> PrepareEditViewModelAsync(int id)
        {
            var menuItem = await _context.MenuItems.FindAsync(id);
            if (menuItem == null)
                return null;

            var viewModel = new MenuItemViewModel
            {
                Id = menuItem.Id,
                Name = menuItem.Name,
                Description = menuItem.Description,
                Price = menuItem.Price,
                DiscountedPrice = menuItem.DiscountedPrice,
                CategoryId = menuItem.CategoryId,
                IsAvailable = menuItem.IsAvailable,
                DisplayOrder = menuItem.DisplayOrder,
                ImageUrl = menuItem.ImageUrl,
                MenuCategories = await GetMenuCategoriesSelectListAsync(menuItem.CategoryId)
            };

            return viewModel;
        }

        public async Task<(bool Success, string Message)> UpdateMenuItemAsync(MenuItemViewModel model, IFormFile imageFile)
        {
            if (model == null)
                return (false, "Model không hợp lệ");

            try
            {
                var menuItem = await _context.MenuItems.FindAsync(model.Id);
                if (menuItem == null)
                    return (false, "Không tìm thấy món ăn");

                menuItem.Name = model.Name;
                menuItem.Description = model.Description;
                menuItem.Price = model.Price;
                menuItem.DiscountedPrice = model.DiscountedPrice;
                menuItem.CategoryId = model.CategoryId;
                menuItem.IsAvailable = model.IsAvailable;
                menuItem.DisplayOrder = model.DisplayOrder;

                // Xử lý tải lên hình ảnh mới
                if (imageFile != null && imageFile.Length > 0)
                {
                    // Xóa hình ảnh cũ nếu có
                    if (!string.IsNullOrEmpty(menuItem.ImageUrl))
                    {
                        var oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", menuItem.ImageUrl.TrimStart('/'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    // Đảm bảo thư mục tồn tại
                    Directory.CreateDirectory(_uploadsFolder);
                    
                    // Tạo tên file duy nhất
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                    var filePath = Path.Combine(_uploadsFolder, fileName);
                    
                    // Lưu file
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                    }
                    
                    // Cập nhật đường dẫn hình ảnh
                    menuItem.ImageUrl = "/uploads/" + fileName;
                }

                _context.Update(menuItem);
                await _context.SaveChangesAsync();

                return (true, "Cập nhật món ăn thành công!");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await MenuItemExistsAsync(model.Id))
                    return (false, "Không tìm thấy món ăn");
                else
                    throw;
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi: {ex.Message}");
            }
        }

        public async Task<MenuItem> GetMenuItemForDeleteAsync(int id)
        {
            return await _context.MenuItems
                .Include(m => m.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<(bool Success, string Message)> DeleteMenuItemAsync(int id)
        {
            try
            {
                // Kiểm tra xem món ăn có trong đơn hàng nào không
                var isInOrder = await _context.OrderItems.AnyAsync(o => o.MenuItemId == id);
                if (isInOrder)
                    return (false, "Không thể xóa món ăn đang được sử dụng trong đơn hàng");

                var menuItem = await _context.MenuItems.FindAsync(id);
                if (menuItem == null)
                    return (false, "Không tìm thấy món ăn");

                // Xóa hình ảnh nếu có
                if (!string.IsNullOrEmpty(menuItem.ImageUrl))
                {
                    var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", menuItem.ImageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                }

                _context.MenuItems.Remove(menuItem);
                await _context.SaveChangesAsync();

                return (true, "Xóa món ăn thành công!");
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi: {ex.Message}");
            }
        }

        public async Task<bool> MenuItemExistsAsync(int id)
        {
            return await _context.MenuItems.AnyAsync(e => e.Id == id);
        }

        public async Task<Dictionary<string, List<MenuItem>>> GetMenuItemsGroupedByCategoryAsync()
        {
            var menuItems = await _context.MenuItems
                .Include(m => m.Category)
                .Where(m => m.IsAvailable)
                .OrderBy(m => m.Category.DisplayOrder)
                .ThenBy(m => m.DisplayOrder)
                .ToListAsync();

            return menuItems
                .GroupBy(m => m.Category?.Name ?? "Không phân loại")
                .ToDictionary(g => g.Key, g => g.ToList());
        }

        public async Task<List<MenuItem>> GetMenuItemsByCategoryIdAsync(int categoryId)
        {
            return await _context.MenuItems
                .Where(m => m.CategoryId == categoryId && m.IsAvailable)
                .OrderBy(m => m.DisplayOrder)
                .ToListAsync();
        }

        public async Task<SelectList> GetMenuCategoriesSelectListAsync(int? selectedCategoryId = null)
        {
            return new SelectList(
                await _context.MenuCategories.OrderBy(c => c.DisplayOrder).ToListAsync(),
                "Id",
                "Name",
                selectedCategoryId
            );
        }

        public async Task<CustomerMenuViewModel> PrepareCustomerMenuViewModelAsync(int? categoryId)
        {
            // Get all categories that are available
            var categories = await _context.MenuCategories
                .Where(c => c.IsAvailable)
                .OrderBy(c => c.DisplayOrder)
                .ToListAsync();

            // Get menu items
            var menuItemsQuery = _context.MenuItems
                .Include(m => m.Category)
                .Where(m => m.IsAvailable);
                
            // Filter by category if specified
            if (categoryId.HasValue)
            {
                menuItemsQuery = menuItemsQuery.Where(m => m.CategoryId == categoryId);
            }
            
            // Get the menu items ordered by category and name
            var menuItems = await menuItemsQuery
                .OrderBy(m => m.Category.DisplayOrder)
                .ThenBy(m => m.Name)
                .ToListAsync();
                
            // Create view model for customer menu display
            var viewModel = new CustomerMenuViewModel
            {
                Categories = categories,
                MenuItems = menuItems,
                SelectedCategoryId = categoryId
            };
            
            return viewModel;
        }

        public async Task<(bool Success, string Message)> ToggleMenuItemStatusAsync(int id)
        {
            try
            {
                var menuItem = await _context.MenuItems.FindAsync(id);
                if (menuItem == null)
                    return (false, "Không tìm thấy món ăn");

                // Đổi trạng thái
                menuItem.IsAvailable = !menuItem.IsAvailable;
                await _context.SaveChangesAsync();
                
                return (true, $"Đã {(menuItem.IsAvailable ? "bật" : "tắt")} phục vụ món ăn!");
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi: {ex.Message}");
            }
        }

        public async Task<(bool Success, string Message)> ToggleMenuItemFeaturedAsync(int id)
        {
            try
            {
                var menuItem = await _context.MenuItems.FindAsync(id);
                if (menuItem == null)
                    return (false, "Không tìm thấy món ăn");

                // Đổi trạng thái nổi bật
                menuItem.IsFeatured = !menuItem.IsFeatured;
                await _context.SaveChangesAsync();
                
                return (true, $"Đã {(menuItem.IsFeatured ? "thêm vào" : "xóa khỏi")} danh sách nổi bật!");
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi: {ex.Message}");
            }
        }
    }
} 