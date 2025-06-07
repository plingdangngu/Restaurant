using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantManagement.Models;
using RestaurantManagement.Services.Interfaces;
using System.Threading.Tasks;

namespace RestaurantManagement.Controllers
{
    [Authorize(Roles = "Admin,Employee")]
    public class MenuCategoryController : Controller
    {
        private readonly IMenuCategoryService _menuCategoryService;

        public MenuCategoryController(IMenuCategoryService menuCategoryService)
        {
            _menuCategoryService = menuCategoryService;
        }

        // GET: MenuCategory
        public async Task<IActionResult> Index()
        {
            var categories = await _menuCategoryService.GetAllCategoriesAsync();
            return View(categories);
        }

        // GET: MenuCategory/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _menuCategoryService.GetCategoryByIdAsync(id.Value);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // GET: MenuCategory/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: MenuCategory/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,IsAvailable,DisplayOrder")] MenuCategory category)
        {
            if (ModelState.IsValid)
            {
                var result = await _menuCategoryService.CreateCategoryAsync(category);
                
                if (result.Success)
                {
                    TempData["SuccessMessage"] = result.Message;
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("", result.Message);
                }
            }
            
            return View(category);
        }

        // GET: MenuCategory/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _menuCategoryService.GetCategoryByIdAsync(id.Value);
            if (category == null)
            {
                return NotFound();
            }
            
            return View(category);
        }

        // POST: MenuCategory/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,IsAvailable,DisplayOrder")] MenuCategory category)
        {
            if (id != category.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var result = await _menuCategoryService.UpdateCategoryAsync(category);
                
                if (result.Success)
                {
                    TempData["SuccessMessage"] = result.Message;
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("", result.Message);
                }
            }
            
            return View(category);
        }

        // GET: MenuCategory/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _menuCategoryService.GetCategoryForDeleteAsync(id.Value);
            if (category == null)
            {
                return NotFound();
            }

            // Thêm thông tin số lượng món ăn trong danh mục
            ViewBag.ItemCount = await _menuCategoryService.GetItemCountByCategoryIdAsync(id.Value);
            
            return View(category);
        }

        // POST: MenuCategory/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _menuCategoryService.DeleteCategoryAsync(id);
            
            if (result.Success)
                TempData["SuccessMessage"] = result.Message;
            else
                TempData["ErrorMessage"] = result.Message;
                
            return RedirectToAction(nameof(Index));
        }

        // GET: MenuCategory/ToggleStatus/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ToggleStatus(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            
            var category = await _menuCategoryService.GetCategoryByIdAsync(id.Value);
            if (category == null)
            {
                return NotFound();
            }
            
            // Đổi trạng thái
            category.IsAvailable = !category.IsAvailable;
            await _menuCategoryService.UpdateCategoryAsync(category);
            
            TempData["SuccessMessage"] = $"Đã {(category.IsAvailable ? "kích hoạt" : "vô hiệu hóa")} danh mục!";
            return RedirectToAction(nameof(Index));
        }
    }
} 