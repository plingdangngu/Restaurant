using System.ComponentModel.DataAnnotations;

namespace RestaurantManagement.ViewModels
{
    public class EditUserViewModel
    {
        public string Id { get; set; } 
        
        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; } 
        
        [Required(ErrorMessage = "Họ tên không được để trống")]
        [Display(Name = "Họ tên")]
        public string FullName { get; set; } 
        
        [Display(Name = "Địa chỉ")]
        public string Address { get; set; } 
        
        [Display(Name = "Trạng thái hoạt động")]
        public bool IsActive { get; set; }
        
        [Display(Name = "Là quản trị viên")]
        public bool IsAdmin { get; set; }
    }
} 