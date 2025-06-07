using System.ComponentModel.DataAnnotations;

namespace RestaurantManagement.ViewModels
{
    public class UserViewModel
    {
        public string? Id { get; set; }
        
        [Required(ErrorMessage = "Họ và tên là bắt buộc")]
        [StringLength(100, ErrorMessage = "Họ và tên không được vượt quá 100 ký tự")]
        [Display(Name = "Họ và tên")]
        public string FullName { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;
        
        [Phone(ErrorMessage = "Số điện thoại không đúng định dạng")]
        [Display(Name = "Số điện thoại")]
        public string? PhoneNumber { get; set; }
        
        [StringLength(200, ErrorMessage = "Địa chỉ không được vượt quá 200 ký tự")]
        [Display(Name = "Địa chỉ")]
        public string? Address { get; set; }
        
        [Display(Name = "Ngày sinh")]
        [DataType(DataType.Date)]
        public DateTime? BirthDate { get; set; }
        
        [Display(Name = "Ngày tham gia")]
        [DataType(DataType.Date)]
        public DateTime JoinDate { get; set; } = DateTime.Now;
        
        [Display(Name = "Còn hoạt động")]
        public bool IsActive { get; set; } = true;
        
        [StringLength(500, ErrorMessage = "Ghi chú không được vượt quá 500 ký tự")]
        [Display(Name = "Ghi chú")]
        public string? Notes { get; set; }
        
        [Display(Name = "Vai trò")]
        public List<string> SelectedRoles { get; set; } = new List<string>();
        
        public List<string> AvailableRoles { get; set; } = new List<string>();
        
        // Chỉ sử dụng khi tạo mới user
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string? Password { get; set; }
        
        [DataType(DataType.Password)]
        [Display(Name = "Xác nhận mật khẩu")]
        [Compare("Password", ErrorMessage = "Mật khẩu và xác nhận mật khẩu không khớp.")]
        public string? ConfirmPassword { get; set; }
    }
} 