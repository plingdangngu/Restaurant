using System.ComponentModel.DataAnnotations;
using RestaurantManagement.Models;

namespace RestaurantManagement.ViewModels
{
    public class ReservationViewModel
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Vui lòng chọn bàn")]
        [Display(Name = "Bàn")]
        public int TableId { get; set; }
        
        [Required(ErrorMessage = "Vui lòng nhập số người")]
        [Display(Name = "Số người")]
        [Range(1, 100, ErrorMessage = "Số người phải từ 1 đến 100")]
        public int PeopleCount { get; set; } = 1;
        
        [Required(ErrorMessage = "Vui lòng chọn thời gian đặt bàn")]
        [Display(Name = "Thời gian đặt bàn")]
        public DateTime ReservationTime { get; set; } = DateTime.Now.AddHours(1);
        
        [Display(Name = "Ngày đặt bàn")]
        public DateTime ReservationDate { get; set; } = DateTime.Today;
        
        [Display(Name = "Tên khách hàng")]
        public string? CustomerName { get; set; }
        
        [Display(Name = "Số điện thoại")]
        public string? CustomerPhone { get; set; }
        
        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string? CustomerEmail { get; set; }
        
        [Display(Name = "Ghi chú")]
        [StringLength(500, ErrorMessage = "Ghi chú không được vượt quá 500 ký tự")]
        public string? Notes { get; set; }
        
        // Danh sách các bàn có sẵn để hiển thị trong dropdown
        public List<Table> AvailableTables { get; set; } = new List<Table>();
    }
} 