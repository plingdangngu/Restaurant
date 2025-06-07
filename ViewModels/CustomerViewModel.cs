using System.ComponentModel.DataAnnotations;
using RestaurantManagement.Models;

namespace RestaurantManagement.ViewModels
{
    public class CustomerViewModel
    {
        public string? Id { get; set; }
        
        [Required(ErrorMessage = "Tên khách hàng là bắt buộc")]
        [StringLength(100, ErrorMessage = "Tên không được vượt quá 100 ký tự")]
        public string Name { get; set; } 
        
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        public string? Email { get; set; }
        
        [Phone(ErrorMessage = "Số điện thoại không đúng định dạng")]
        public string? Phone { get; set; }
        
        [StringLength(200, ErrorMessage = "Địa chỉ không được vượt quá 200 ký tự")]
        public string? Address { get; set; }
        
        public string? Notes { get; set; }
    }

    public class CustomerDetailsViewModel
    {
        public string Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? Notes { get; set; }
        public int OrderCount { get; set; }
        public decimal TotalSpent { get; set; }
        public CustomerViewModel Customer { get; set; } = new();
        public List<OrderViewModel> Orders { get; set; } = new();
        public List<DashboardReservationViewModel> Reservations { get; set; } = new();
        public List<Invoice> Invoices { get; set; } = new();
    }
} 