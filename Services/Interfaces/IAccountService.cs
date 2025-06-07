using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using RestaurantManagement.Models;
using RestaurantManagement.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RestaurantManagement.Services.Interfaces
{
    public interface IAccountService
    {
        Task<(bool Success, string Message)> RegisterAsync(RegisterViewModel model);
        Task<(bool Success, string Message, ApplicationUser User)> LoginAsync(LoginViewModel model);
        Task LogoutAsync();
        Task<(bool Success, string Message)> ChangePasswordAsync(string userId, ChangePasswordViewModel model);
        Task<(bool Success, string Message)> ResetPasswordAsync(string userId, string newPassword);
        Task<ApplicationUser> GetCurrentUserAsync();
        Task<bool> IsInRoleAsync(ApplicationUser user, string role);
        Task<bool> IsEmailInUseAsync(string email);
        Task<ProfileViewModel> GetProfileViewModelAsync(string userId);
        Task<(bool Success, string Message)> UpdateProfileAsync(ProfileViewModel model);
        Task<IEnumerable<AuthenticationScheme>> GetExternalAuthenticationSchemesAsync();
        Task<(bool Success, string Error)> ExternalLoginAsync(string provider, string returnUrl);
        Task<(bool Success, string Error, ExternalLoginInfo Info)> GetExternalLoginInfoAsync();
        Task<(bool Success, string Message, ApplicationUser User)> ExternalLoginConfirmAsync(ExternalLoginViewModel model);
    }
} 