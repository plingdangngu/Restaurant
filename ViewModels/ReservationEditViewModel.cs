using System.ComponentModel.DataAnnotations;

namespace RestaurantManagement.ViewModels
{
    public class ReservationEditViewModel
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Vui lòng chọn bàn")]
        [Display(Name = "Bàn")]
        public int TableId { get; set; }
        
        [Required(ErrorMessage = "Vui lòng chọn khách hàng")]
        [Display(Name = "Khách hàng")]
        public string CustomerId { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Vui lòng nhập số người")]
        [Display(Name = "Số người")]
        [Range(1, 100, ErrorMessage = "Số người phải từ 1 đến 100")]
        public int PeopleCount { get; set; } = 1;
        
        [Display(Name = "Ghi chú")]
        [StringLength(500, ErrorMessage = "Ghi chú không được vượt quá 500 ký tự")]
        public string? Notes { get; set; }
        
        [Required(ErrorMessage = "Vui lòng chọn trạng thái")]
        [Display(Name = "Trạng thái")]
        public string Status { get; set; } = "Pending";
        
        // Constructor với giá trị mặc định
        public ReservationEditViewModel()
        {
            CustomerId = string.Empty;
            Status = "Pending";
        }
    }
} 