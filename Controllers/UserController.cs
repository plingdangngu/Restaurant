using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Models;
using System.Threading.Tasks;

namespace RestaurantManagement.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        
        public UserController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        
        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            return View(users);
        }
        
        public IActionResult Create()
        {
            return RedirectToAction("Register", "Account");
        }
        
        public async Task<IActionResult> Details(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            
            var roles = await _userManager.GetRolesAsync(user);
            
            ViewData["Roles"] = string.Join(", ", roles);
            
            return View(user);
        }
        
        public IActionResult Edit(string id)
        {
            return RedirectToAction("Edit", "Account", new { id });
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            
            // Không xóa tài khoản khỏi DB mà chỉ vô hiệu hóa
            user.IsActive = false;
            await _userManager.UpdateAsync(user);
            
            TempData["SuccessMessage"] = "Đã vô hiệu hóa tài khoản thành công!";
            return RedirectToAction(nameof(Index));
        }
    }
} 