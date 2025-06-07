using System.ComponentModel.DataAnnotations;

namespace RestaurantManagement.ViewModels
{
    public class ProfileViewModel
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên")]
        [Display(Name = "Họ và tên")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [Display(Name = "Số điện thoại")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Ảnh đại diện")]
        public string ProfilePicture { get; set; }
    }
} 