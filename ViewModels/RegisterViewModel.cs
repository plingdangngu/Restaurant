using System.ComponentModel.DataAnnotations;

namespace RestaurantManagement.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        [StringLength(100, ErrorMessage = "Mật khẩu phải có ít nhất {2} ký tự và tối đa {1} ký tự", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Xác nhận mật khẩu không được để trống")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Mật khẩu và xác nhận mật khẩu không khớp")]
        [Display(Name = "Xác nhận mật khẩu")]
        public string ConfirmPassword { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Họ tên không được để trống")]
        [Display(Name = "Họ tên")]
        public string FullName { get; set; } = string.Empty;
        
        [Display(Name = "Địa chỉ")]
        public string Address { get; set; } = string.Empty;
        
        [Display(Name = "Là quản trị viên")]
        public bool IsAdmin { get; set; }
        
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [Display(Name = "Số điện thoại")]
        public string PhoneNumber { get; set; } = string.Empty;
        
        [Display(Name = "Vai trò")]
        public string Role { get; set; } = "Customer";
    }
} 