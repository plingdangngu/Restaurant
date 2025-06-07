using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Models;
using RestaurantManagement.Services.Interfaces;
using RestaurantManagement.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantManagement.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<List<ApplicationUser>> GetAllUsersAsync()
        {
            return await _userManager.Users
                .OrderBy(u => u.UserName)
                .ToListAsync();
        }

        public async Task<ApplicationUser> GetUserByIdAsync(string id)
        {
            return await _userManager.FindByIdAsync(id);
        }

        public async Task<UserViewModel> PrepareCreateViewModelAsync()
        {
            var viewModel = new UserViewModel
            {
                JoinDate = DateTime.Now,
                IsActive = true,
                AvailableRoles = (await GetAllRolesAsync()).Select(r => r.Name).ToList()
            };
            
            return viewModel;
        }

        public async Task<(bool Success, string Message, ApplicationUser User)> CreateUserAsync(UserViewModel model)
        {
            if (model == null)
                return (false, "Model không hợp lệ", null);

            try
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FullName = model.FullName,
                    PhoneNumber = model.PhoneNumber,
                    Address = model.Address,
                    BirthDate = model.BirthDate,
                    JoinDate = model.JoinDate,
                    IsActive = model.IsActive,
                    Notes = model.Notes
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    if (model.SelectedRoles != null && model.SelectedRoles.Any())
                    {
                        await _userManager.AddToRolesAsync(user, model.SelectedRoles);
                    }

                    return (true, "Thêm người dùng thành công!", user);
                }
                
                string errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return (false, $"Lỗi: {errors}", null);
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi: {ex.Message}", null);
            }
        }

        public async Task<UserViewModel> PrepareEditViewModelAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return null;
                
            var userRoles = await _userManager.GetRolesAsync(user);
            var allRoles = await _roleManager.Roles.Select(r => r.Name).ToListAsync();
            
            var viewModel = new UserViewModel
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address,
                BirthDate = user.BirthDate,
                JoinDate = user.JoinDate,
                IsActive = user.IsActive,
                Notes = user.Notes,
                SelectedRoles = userRoles.ToList(),
                AvailableRoles = allRoles
            };
            
            return viewModel;
        }

        public async Task<(bool Success, string Message)> UpdateUserAsync(UserViewModel model)
        {
            if (model == null)
                return (false, "Model không hợp lệ");

            try
            {
                var user = await _userManager.FindByIdAsync(model.Id);
                if (user == null)
                    return (false, "Không tìm thấy người dùng");

                user.FullName = model.FullName;
                user.PhoneNumber = model.PhoneNumber;
                user.Email = model.Email;
                user.UserName = model.Email;
                user.Address = model.Address;
                user.BirthDate = model.BirthDate;
                user.JoinDate = model.JoinDate;
                user.IsActive = model.IsActive;
                user.Notes = model.Notes;

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    // Cập nhật vai trò
                    if (model.SelectedRoles != null)
                    {
                        await ChangeUserRolesAsync(user.Id, model.SelectedRoles);
                    }
                    
                    return (true, "Cập nhật người dùng thành công!");
                }
                
                string errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return (false, $"Lỗi: {errors}");
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi: {ex.Message}");
            }
        }

        public async Task<ApplicationUser> GetUserForDeleteAsync(string id)
        {
            return await _userManager.FindByIdAsync(id);
        }

        public async Task<(bool Success, string Message)> DeleteUserAsync(string id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                    return (false, "Không tìm thấy người dùng");

                var result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    return (true, "Xóa người dùng thành công!");
                }
                
                string errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return (false, $"Lỗi: {errors}");
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi: {ex.Message}");
            }
        }

        public async Task<bool> UserExistsAsync(string id)
        {
            return await _userManager.FindByIdAsync(id) != null;
        }

        public async Task<List<string>> GetUserRolesAsync(ApplicationUser user)
        {
            if (user == null)
                return new List<string>();
                
            return (await _userManager.GetRolesAsync(user)).ToList();
        }

        public async Task<List<IdentityRole>> GetAllRolesAsync()
        {
            return await _roleManager.Roles.OrderBy(r => r.Name).ToListAsync();
        }

        public async Task<(bool Success, string Message)> ChangeUserRolesAsync(string userId, List<string> selectedRoles)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return (false, "Không tìm thấy người dùng");
                
            var currentRoles = await _userManager.GetRolesAsync(user);
            
            // Xóa các vai trò hiện tại
            var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
            if (!removeResult.Succeeded)
            {
                string errors = string.Join(", ", removeResult.Errors.Select(e => e.Description));
                return (false, $"Lỗi khi xóa vai trò: {errors}");
            }
            
            // Thêm các vai trò mới
            if (selectedRoles != null && selectedRoles.Any())
            {
                var addResult = await _userManager.AddToRolesAsync(user, selectedRoles);
                if (!addResult.Succeeded)
                {
                    string errors = string.Join(", ", addResult.Errors.Select(e => e.Description));
                    return (false, $"Lỗi khi thêm vai trò: {errors}");
                }
            }
            
            return (true, "Cập nhật vai trò thành công!");
        }

        public async Task<List<IdentityRole>> GetRolesAsync()
        {
            return await _roleManager.Roles.OrderBy(r => r.Name).ToListAsync();
        }

        public async Task<(bool Success, string Message)> AssignUserToRoleAsync(string userId, string role)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return (false, "Không tìm thấy người dùng");

            if (!await _roleManager.RoleExistsAsync(role))
                return (false, "Vai trò không tồn tại");

            if (await _userManager.IsInRoleAsync(user, role))
                return (true, $"Người dùng đã có vai trò {role}");

            var result = await _userManager.AddToRoleAsync(user, role);
            if (result.Succeeded)
            {
                return (true, $"Đã thêm người dùng vào vai trò {role}!");
            }
            
            string errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return (false, $"Lỗi: {errors}");
        }

        public async Task<(bool Success, string Message)> RemoveUserFromRoleAsync(string userId, string role)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return (false, "Không tìm thấy người dùng");

            if (!await _userManager.IsInRoleAsync(user, role))
                return (true, $"Người dùng không có vai trò {role}");

            var result = await _userManager.RemoveFromRoleAsync(user, role);
            if (result.Succeeded)
            {
                return (true, $"Đã xóa người dùng khỏi vai trò {role}!");
            }
            
            string errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return (false, $"Lỗi: {errors}");
        }

        public async Task<List<string>> GetUserRolesAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return new List<string>();

            return (await _userManager.GetRolesAsync(user)).ToList();
        }

        public async Task<bool> IsUserInRoleAsync(string userId, string role)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return false;

            return await _userManager.IsInRoleAsync(user, role);
        }

        public async Task<List<ApplicationUser>> GetUsersInRoleAsync(string role)
        {
            return (await _userManager.GetUsersInRoleAsync(role)).ToList();
        }

        public async Task<SelectList> GetUsersInRoleSelectListAsync(string role, string selectedUserId = null)
        {
            var users = await GetUsersInRoleAsync(role);
            
            return new SelectList(
                users.Select(u => new
                {
                    Id = u.Id,
                    Name = u.FullName ?? u.UserName
                }),
                "Id",
                "Name",
                selectedUserId
            );
        }

        public async Task<string> GetUserNameAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            return user?.UserName;
        }
    }
} 