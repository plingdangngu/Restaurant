using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestaurantManagement.Models;
using RestaurantManagement.Services.Interfaces;
using RestaurantManagement.ViewModels;
using System.Threading.Tasks;

namespace RestaurantManagement.Controllers
{
    [Authorize]
    public class MenuItemController : Controller
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IMenuItemService _menuItemService;
        private readonly IMenuCategoryService _menuCategoryService;

        public MenuItemController(IWebHostEnvironment hostingEnvironment, IMenuItemService menuItemService, IMenuCategoryService menuCategoryService)
        {
            _hostingEnvironment = hostingEnvironment;
            _menuItemService = menuItemService;
            _menuCategoryService = menuCategoryService;
        }

        // GET: MenuItem/CustomerMenu
        [AllowAnonymous]
        public async Task<IActionResult> CustomerMenu(int? categoryId)
        {
            var viewModel = await _menuItemService.PrepareCustomerMenuViewModelAsync(categoryId);
            return View(viewModel);
        }

        // GET: MenuItem
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> Index()
        {
            var menuItems = await _menuItemService.GetAllMenuItemsAsync();
            return View(menuItems);
        }

        // GET: MenuItem/Details/5
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var menuItem = await _menuItemService.GetMenuItemByIdAsync(id.Value);
            if (menuItem == null)
            {
                return NotFound();
            }

            return View(menuItem);
        }

        // GET: MenuItem/Create
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> Create()
        {
            var viewModel = await _menuItemService.PrepareCreateViewModelAsync();
            return View(viewModel);
        }

        // POST: MenuItem/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> Create(MenuItemViewModel viewModel, IFormFile imageFile)
        {
            if (ModelState.IsValid)
            {
                var result = await _menuItemService.CreateMenuItemAsync(viewModel, imageFile);
                
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
            
            viewModel.MenuCategories = await _menuItemService.GetMenuCategoriesSelectListAsync(viewModel.CategoryId);
            return View(viewModel);
        }

        // GET: MenuItem/Edit/5
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var viewModel = await _menuItemService.PrepareEditViewModelAsync(id.Value);
            if (viewModel == null)
            {
                return NotFound();
            }
            
            return View(viewModel);
        }

        // POST: MenuItem/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> Edit(int id, MenuItemViewModel viewModel, IFormFile imageFile)
        {
            if (id != viewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var result = await _menuItemService.UpdateMenuItemAsync(viewModel, imageFile);
                
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
            
            viewModel.MenuCategories = await _menuItemService.GetMenuCategoriesSelectListAsync(viewModel.CategoryId);
            return View(viewModel);
        }

        // GET: MenuItem/Delete/5
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var menuItem = await _menuItemService.GetMenuItemForDeleteAsync(id.Value);
            if (menuItem == null)
            {
                return NotFound();
            }

            return View(menuItem);
        }

        // POST: MenuItem/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _menuItemService.DeleteMenuItemAsync(id);
            
            if (result.Success)
                TempData["SuccessMessage"] = result.Message;
            else
                TempData["ErrorMessage"] = result.Message;
                
            return RedirectToAction(nameof(Index));
        }

        // GET: MenuItem/ToggleStatus/5
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> ToggleStatus(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var result = await _menuItemService.ToggleMenuItemStatusAsync(id.Value);
            
            if (result.Success)
                TempData["SuccessMessage"] = result.Message;
            else
                TempData["ErrorMessage"] = result.Message;
            
            // Nếu từ trang chi tiết danh mục, trở về trang đó
            if (Request.Headers["Referer"].ToString().Contains("MenuCategory/Details"))
            {
                return Redirect(Request.Headers["Referer"].ToString());
            }
            
            return RedirectToAction(nameof(Index));
        }

        // GET: MenuItem/ToggleFeatured/5
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> ToggleFeatured(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var result = await _menuItemService.ToggleMenuItemFeaturedAsync(id.Value);
            
            if (result.Success)
                TempData["SuccessMessage"] = result.Message;
            else
                TempData["ErrorMessage"] = result.Message;
            
            // Nếu từ trang chi tiết danh mục, trở về trang đó
            if (Request.Headers["Referer"].ToString().Contains("MenuCategory/Details"))
            {
                return Redirect(Request.Headers["Referer"].ToString());
            }
            
            return RedirectToAction(nameof(Index));
        }

        // GET: MenuItem/ByCategory/5
        [AllowAnonymous]
        public async Task<IActionResult> ByCategory(int id)
        {
            var menuItems = await _menuItemService.GetMenuItemsByCategoryIdAsync(id);
            return View(menuItems);
        }
        
        // GET: MenuItem/Menu
        [AllowAnonymous]
        public async Task<IActionResult> Menu()
        {
            var menuItemsByCategory = await _menuItemService.GetMenuItemsGroupedByCategoryAsync();
            return View(menuItemsByCategory);
        }
    }
} 