using Microsoft.AspNetCore.Identity;
using RestaurantManagement.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RestaurantManagement.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<List<ApplicationUser>> GetAllAsync();
        Task<ApplicationUser> GetByIdAsync(string id);
        Task<ApplicationUser> FindByEmailAsync(string email);
        Task<ApplicationUser> FindByNameAsync(string userName);
        Task<bool> ExistsAsync(string id);
        Task<bool> ExistsByEmailAsync(string email);
        Task<bool> IsInRoleAsync(ApplicationUser user, string role);
        Task<List<string>> GetRolesAsync(ApplicationUser user);
        Task<IdentityResult> AddToRoleAsync(ApplicationUser user, string role);
        Task<IdentityResult> RemoveFromRoleAsync(ApplicationUser user, string role);
        Task<IdentityResult> CreateAsync(ApplicationUser user, string password);
        Task<IdentityResult> UpdateAsync(ApplicationUser user);
        Task<IdentityResult> DeleteAsync(ApplicationUser user);
        Task<IdentityResult> ResetPasswordAsync(ApplicationUser user, string token, string newPassword);
        Task<string> GeneratePasswordResetTokenAsync(ApplicationUser user);
        Task<List<IdentityRole>> GetRolesListAsync();
        Task SaveChangesAsync();
    }
} 