using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using RestaurantManagement.Models;
using RestaurantManagement.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RestaurantManagement.Services.Interfaces
{
    public interface IUserService
    {
        Task<List<ApplicationUser>> GetAllUsersAsync();
        Task<ApplicationUser> GetUserByIdAsync(string id);
        Task<UserViewModel> PrepareCreateViewModelAsync();
        Task<(bool Success, string Message, ApplicationUser User)> CreateUserAsync(UserViewModel model);
        Task<UserViewModel> PrepareEditViewModelAsync(string id);
        Task<(bool Success, string Message)> UpdateUserAsync(UserViewModel model);
        Task<ApplicationUser> GetUserForDeleteAsync(string id);
        Task<(bool Success, string Message)> DeleteUserAsync(string id);
        Task<bool> UserExistsAsync(string id);
        Task<List<string>> GetUserRolesAsync(ApplicationUser user);
        Task<List<IdentityRole>> GetAllRolesAsync();
        Task<(bool Success, string Message)> ChangeUserRolesAsync(string userId, List<string> selectedRoles);
        Task<SelectList> GetUsersInRoleSelectListAsync(string role, string selectedUserId = null);
        Task<List<IdentityRole>> GetRolesAsync();
        Task<(bool Success, string Message)> AssignUserToRoleAsync(string userId, string role);
        Task<(bool Success, string Message)> RemoveUserFromRoleAsync(string userId, string role);
        Task<List<string>> GetUserRolesAsync(string userId);
        Task<bool> IsUserInRoleAsync(string userId, string role);
        Task<List<ApplicationUser>> GetUsersInRoleAsync(string role);
        Task<string> GetUserNameAsync(string userId);
    }
} 