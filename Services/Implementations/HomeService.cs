using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RestaurantManagement.Models;
using RestaurantManagement.Services.Interfaces;
using System.Diagnostics;
using System.Threading.Tasks;

namespace RestaurantManagement.Services.Implementations
{
    public class HomeService : IHomeService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public Task<IActionResult> GetHomePageData()
        {
            // Trả về dữ liệu cho trang chủ
            // Có thể lấy dữ liệu từ database nếu cần
            return Task.FromResult<IActionResult>(new ViewResult());
        }

        public Task<IActionResult> GetPrivacyData()
        {
            // Trả về dữ liệu cho trang privacy
            return Task.FromResult<IActionResult>(new ViewResult());
        }

        public Task<ErrorViewModel> GetErrorViewModel(string requestId)
        {
            return Task.FromResult(new ErrorViewModel 
            { 
                RequestId = requestId ?? Activity.Current?.Id ?? "Không có mã yêu cầu"
            });
        }

        public async Task<IActionResult> RedirectBasedOnUserRole(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return new RedirectToActionResult("Index", "Home", null);

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return new RedirectToActionResult("Index", "Home", null);

            bool isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
            bool isEmployee = await _userManager.IsInRoleAsync(user, "Employee");

            if (isAdmin || isEmployee)
                return new RedirectToActionResult("Index", "Dashboard", null);
            else
                return new RedirectToActionResult("Index", "Home", null);
        }
    }
} 