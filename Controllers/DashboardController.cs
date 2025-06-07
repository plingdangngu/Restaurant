using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantManagement.Models.Dashboard;
using RestaurantManagement.Services.Interfaces;
using System.Threading.Tasks;

namespace RestaurantManagement.Controllers
{
    [Authorize(Roles = "Admin,Employee")]
    public class DashboardController : Controller
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        // GET: Dashboard
        public async Task<IActionResult> Index()
        {
            var dashboardData = await _dashboardService.GetDashboardDataAsync();
            return View(dashboardData);
        }
    }
} 