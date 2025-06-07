using Microsoft.AspNetCore.Mvc;
using RestaurantManagement.Models;
using System.Threading.Tasks;

namespace RestaurantManagement.Services.Interfaces
{
    public interface IHomeService
    {
        Task<IActionResult> GetHomePageData();
        Task<IActionResult> GetPrivacyData();
        Task<ErrorViewModel> GetErrorViewModel(string requestId);
        Task<IActionResult> RedirectBasedOnUserRole(string userId);
    }
} 