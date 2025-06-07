using Microsoft.AspNetCore.Identity;
using RestaurantManagement.Models;
using System.Threading.Tasks;

namespace RestaurantManagement.Repositories.Interfaces
{
    public interface IAccountRepository
    {
        Task<SignInResult> PasswordSignInAsync(string email, string password, bool rememberMe, bool lockoutOnFailure);
        Task SignOutAsync();
        Task<ApplicationUser> FindByEmailAsync(string email);
        Task<IdentityResult> CreateUserAsync(ApplicationUser user, string password);
        Task<IdentityResult> AddToRoleAsync(ApplicationUser user, string role);
        Task<bool> IsInRoleAsync(ApplicationUser user, string role);
        Task<string> GenerateEmailConfirmationTokenAsync(ApplicationUser user);
        Task<IdentityResult> ConfirmEmailAsync(ApplicationUser user, string token);
        Task<string> GeneratePasswordResetTokenAsync(ApplicationUser user);
        Task<IdentityResult> ResetPasswordAsync(ApplicationUser user, string token, string newPassword);
        Task<IdentityResult> ChangePasswordAsync(ApplicationUser user, string currentPassword, string newPassword);
    }
} 