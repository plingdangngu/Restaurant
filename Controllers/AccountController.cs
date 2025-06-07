using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RestaurantManagement.Models;
using RestaurantManagement.ViewModels;

namespace RestaurantManagement.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        
        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }
        
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: true);
                
                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(returnUrl))
                    {
                        return LocalRedirect(returnUrl);
                    }
                    
                    var user = await _userManager.FindByEmailAsync(model.Email);
                    
                    if (await _userManager.IsInRoleAsync(user, "Admin") || 
                        await _userManager.IsInRoleAsync(user, "Employee"))
                    {
                        return RedirectToAction("Index", "Dashboard");
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                
                if (result.IsLockedOut)
                {
                    ModelState.AddModelError(string.Empty, "Tài khoản đã bị khóa. Vui lòng thử lại sau.");
                    return View(model);
                }
                
                ModelState.AddModelError(string.Empty, "Email hoặc mật khẩu không đúng.");
            }
            
            return View(model);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
        
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FullName = model.FullName,
                    Address = model.Address,
                    JoinDate = DateTime.Now,
                    IsActive = true,
                    EmailConfirmed = true
                };
                
                var result = await _userManager.CreateAsync(user, model.Password);
                
                if (result.Succeeded)
                {
                    // Đăng ký người dùng thông thường với vai trò "User"
                    await _userManager.AddToRoleAsync(user, "User");
                    
                    // Đăng nhập người dùng sau khi đăng ký
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    
                    TempData["SuccessMessage"] = "Đăng ký tài khoản thành công!";
                    return RedirectToAction("Index", "Home");
                }
                
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            
            return View(model);
        }
        
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            
            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
            
            var model = new EditUserViewModel
            {
                Id = user.Id,
                Email = user.Email!,
                FullName = user.FullName,
                Address = user.Address,
                IsActive = user.IsActive,
                IsAdmin = isAdmin
            };
            
            return View(model);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(EditUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(model.Id);
                if (user == null)
                {
                    return NotFound();
                }
                
                user.Email = model.Email;
                user.UserName = model.Email;
                user.FullName = model.FullName;
                user.Address = model.Address;
                user.IsActive = model.IsActive;
                
                var result = await _userManager.UpdateAsync(user);
                
                if (result.Succeeded)
                {
                    var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
                    
                    if (model.IsAdmin && !isAdmin)
                    {
                        await _userManager.RemoveFromRoleAsync(user, "Employee");
                        await _userManager.AddToRoleAsync(user, "Admin");
                    }
                    else if (!model.IsAdmin && isAdmin)
                    {
                        await _userManager.RemoveFromRoleAsync(user, "Admin");
                        await _userManager.AddToRoleAsync(user, "Employee");
                    }
                    
                    TempData["SuccessMessage"] = "Cập nhật thông tin thành công!";
                    return RedirectToAction("Index", "User");
                }
                
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            
            return View(model);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ToggleStatus(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            
            user.IsActive = !user.IsActive;
            
            await _userManager.UpdateAsync(user);
            
            TempData["SuccessMessage"] = user.IsActive
                ? "Đã kích hoạt tài khoản thành công!"
                : "Đã khóa tài khoản thành công!";
                
            return RedirectToAction("Index", "User");
        }
        
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
} 