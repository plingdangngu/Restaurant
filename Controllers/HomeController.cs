using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RestaurantManagement.Models;
using RestaurantManagement.Services.Interfaces;
using System.Diagnostics;
using System.Threading.Tasks;

namespace RestaurantManagement.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHomeService _homeService;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(IHomeService homeService, UserManager<ApplicationUser> userManager)
        {
            _homeService = homeService;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        // Phương thức để chuyển hướng dựa trên vai trò
        public async Task<IActionResult> RedirectToHome()
        {
            var userId = _userManager.GetUserId(User);
            return await _homeService.RedirectBasedOnUserRole(userId);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            return View(_homeService.GetErrorViewModel(requestId).Result);
        }
    }
} 