using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using RestaurantManagement.Models;
using RestaurantManagement.Services.Interfaces;
using RestaurantManagement.ViewModels;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using System.Collections.Generic;
using System.Linq;

namespace RestaurantManagement.Services.Implementations
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<AccountService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AccountService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<AccountService> logger,
            IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<(bool Success, string Message)> RegisterAsync(RegisterViewModel model)
        {
            if (model == null)
                return (false, "Model không hợp lệ");

            try
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FullName = model.FullName,
                    PhoneNumber = model.PhoneNumber
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("Người dùng đã tạo tài khoản mới.");

                    if (!string.IsNullOrEmpty(model.Role))
                    {
                        await _userManager.AddToRoleAsync(user, model.Role);
                    }
                    else
                    {
                        // Gán vai trò mặc định nếu cần
                        await _userManager.AddToRoleAsync(user, "Customer");
                    }

                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return (true, "Đăng ký thành công!");
                }
                
                string errors = string.Join(", ", result.Errors);
                return (false, $"Lỗi: {errors}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi trong quá trình đăng ký người dùng");
                return (false, $"Lỗi: {ex.Message}");
            }
        }

        public async Task<(bool Success, string Message, ApplicationUser User)> LoginAsync(LoginViewModel model)
        {
            try
            {
                var result = await _signInManager.PasswordSignInAsync(
                    model.Email,
                    model.Password,
                    model.RememberMe,
                    lockoutOnFailure: true);

                if (result.Succeeded)
                {
                    _logger.LogInformation("Người dùng đã đăng nhập.");
                    var user = await _userManager.FindByEmailAsync(model.Email);
                    return (true, "Đăng nhập thành công!", user);
                }
                
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("Tài khoản người dùng bị khóa.");
                    return (false, "Tài khoản đã bị khóa. Vui lòng thử lại sau.", null);
                }
                else
                {
                    return (false, "Đăng nhập không thành công. Vui lòng kiểm tra lại thông tin đăng nhập.", null);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi trong quá trình đăng nhập");
                return (false, $"Lỗi: {ex.Message}", null);
            }
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("Người dùng đã đăng xuất.");
        }

        public async Task<(bool Success, string Message)> ChangePasswordAsync(string userId, ChangePasswordViewModel model)
        {
            if (model == null)
                return (false, "Model không hợp lệ");

            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                    return (false, "Không tìm thấy người dùng");

                var result = await _userManager.ChangePasswordAsync(
                    user,
                    model.OldPassword,
                    model.NewPassword);

                if (result.Succeeded)
                {
                    _logger.LogInformation("Người dùng đã thay đổi mật khẩu thành công.");
                    // Đăng nhập lại với mật khẩu mới
                    await _signInManager.RefreshSignInAsync(user);
                    return (true, "Thay đổi mật khẩu thành công!");
                }
                
                string errors = string.Join(", ", result.Errors);
                return (false, $"Lỗi: {errors}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi trong quá trình thay đổi mật khẩu");
                return (false, $"Lỗi: {ex.Message}");
            }
        }

        public async Task<(bool Success, string Message)> ResetPasswordAsync(string userId, string newPassword)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                    return (false, "Không tìm thấy người dùng");

                // Tạo token đặt lại mật khẩu
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                
                // Đặt lại mật khẩu
                var result = await _userManager.ResetPasswordAsync(user, token, newPassword);

                if (result.Succeeded)
                {
                    _logger.LogInformation("Đã đặt lại mật khẩu cho người dùng.");
                    return (true, "Đặt lại mật khẩu thành công!");
                }
                
                string errors = string.Join(", ", result.Errors);
                return (false, $"Lỗi: {errors}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi trong quá trình đặt lại mật khẩu");
                return (false, $"Lỗi: {ex.Message}");
            }
        }

        public async Task<ApplicationUser> GetCurrentUserAsync()
        {
            return await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
        }

        public async Task<bool> IsInRoleAsync(ApplicationUser user, string role)
        {
            return user != null && await _userManager.IsInRoleAsync(user, role);
        }
        
        // Implement missing methods
        
        public async Task<bool> IsEmailInUseAsync(string email)
        {
            if (string.IsNullOrEmpty(email))
                return false;
                
            var user = await _userManager.FindByEmailAsync(email);
            return user != null;
        }
        
        public async Task<ProfileViewModel> GetProfileViewModelAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return null;
                
            return new ProfileViewModel
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                ProfilePicture = string.Empty // Thay thế bằng giá trị mặc định
            };
        }
        
        public async Task<(bool Success, string Message)> UpdateProfileAsync(ProfileViewModel model)
        {
            if (model == null)
                return (false, "Model không hợp lệ");

            try
            {
                var user = await _userManager.FindByIdAsync(model.Id);
                if (user == null)
                    return (false, "Không tìm thấy người dùng");

                // Check if email is already in use by another user
                if (user.Email != model.Email)
                {
                    var existingUser = await _userManager.FindByEmailAsync(model.Email);
                    if (existingUser != null && existingUser.Id != user.Id)
                        return (false, "Email này đã được sử dụng bởi tài khoản khác");
                        
                    user.Email = model.Email;
                    user.UserName = model.Email; // Update username if it's the same as email
                }
                
                user.FullName = model.FullName;
                user.PhoneNumber = model.PhoneNumber;

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    await _signInManager.RefreshSignInAsync(user);
                    return (true, "Cập nhật thông tin thành công!");
                }
                
                string errors = string.Join(", ", result.Errors);
                return (false, $"Lỗi: {errors}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi trong quá trình cập nhật thông tin");
                return (false, $"Lỗi: {ex.Message}");
            }
        }
        
        public async Task<IEnumerable<AuthenticationScheme>> GetExternalAuthenticationSchemesAsync()
        {
            return await _signInManager.GetExternalAuthenticationSchemesAsync();
        }
        
        public async Task<(bool Success, string Error)> ExternalLoginAsync(string provider, string returnUrl)
        {
            try
            {
                // Store the return URL for later use
                returnUrl = returnUrl ?? "/";
                
                // Request a redirect to the external login provider
                var redirectUrl = $"/Account/ExternalLoginCallback?returnUrl={returnUrl}";
                var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
                
                // This will be handled by the controller that calls this method
                return (true, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi trong quá trình đăng nhập bên ngoài");
                return (false, ex.Message);
            }
        }
        
        public async Task<(bool Success, string Error, ExternalLoginInfo Info)> GetExternalLoginInfoAsync()
        {
            try
            {
                var info = await _signInManager.GetExternalLoginInfoAsync();
                if (info == null)
                    return (false, "Không thể tải thông tin đăng nhập từ dịch vụ ngoài.", null);
                
                return (true, null, info);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi trong quá trình lấy thông tin đăng nhập bên ngoài");
                return (false, ex.Message, null);
            }
        }
        
        public async Task<(bool Success, string Message, ApplicationUser User)> ExternalLoginConfirmAsync(ExternalLoginViewModel model)
        {
            if (model == null)
                return (false, "Model không hợp lệ", null);
                
            try
            {
                // Lấy thông tin đăng nhập bên ngoài
                var info = await _signInManager.GetExternalLoginInfoAsync();
                if (info == null)
                    return (false, "Không thể tải thông tin đăng nhập từ dịch vụ ngoài.", null);
                
                // Kiểm tra email đã được sử dụng chưa
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    // User đã tồn tại, liên kết tài khoản login ngoài
                    var result = await _userManager.AddLoginAsync(user, info);
                    if (result.Succeeded)
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return (true, "Liên kết tài khoản thành công!", user);
                    }
                    
                    string errors = string.Join(", ", result.Errors);
                    return (false, $"Lỗi khi liên kết tài khoản: {errors}", null);
                }
                
                // Tạo tài khoản mới
                user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FullName = model.FullName
                };
                
                var createResult = await _userManager.CreateAsync(user);
                if (createResult.Succeeded)
                {
                    // Gán vai trò mặc định
                    await _userManager.AddToRoleAsync(user, "Customer");
                    
                    // Liên kết tài khoản login ngoài
                    var addLoginResult = await _userManager.AddLoginAsync(user, info);
                    if (addLoginResult.Succeeded)
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return (true, "Tài khoản được tạo và liên kết thành công!", user);
                    }
                    
                    // Xóa tài khoản nếu không liên kết được
                    await _userManager.DeleteAsync(user);
                    string addLoginErrors = string.Join(", ", addLoginResult.Errors);
                    return (false, $"Lỗi khi liên kết tài khoản: {addLoginErrors}", null);
                }
                
                string createErrors = string.Join(", ", createResult.Errors);
                return (false, $"Lỗi khi tạo tài khoản: {createErrors}", null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi trong quá trình xác nhận đăng nhập bên ngoài");
                return (false, $"Lỗi: {ex.Message}", null);
            }
        }
    }
} 