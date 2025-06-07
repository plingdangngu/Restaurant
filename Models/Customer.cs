using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestaurantManagement.Models
{
    public class Customer
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        
        [Required(ErrorMessage = "Vui lòng nhập tên khách hàng")]
        [StringLength(100, ErrorMessage = "Tên khách hàng không được vượt quá 100 ký tự")]
        public string Name { get; set; } = string.Empty;
        
        // Thêm property alias cho Name
        [NotMapped]
        public string FullName { get => Name; set => Name = value; }
        
        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [StringLength(15, ErrorMessage = "Số điện thoại không được vượt quá 15 ký tự")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string Phone { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [StringLength(15, ErrorMessage = "Số điện thoại không được vượt quá 15 ký tự")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string PhoneNumber { get => Phone; set => Phone = value; }
        
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string? Email { get; set; }
        
        [StringLength(200, ErrorMessage = "Địa chỉ không được vượt quá 200 ký tự")]
        public string? Address { get; set; }
        
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        
        [StringLength(500, ErrorMessage = "Ghi chú không được vượt quá 500 ký tự")]
        public string? Notes { get; set; }
        
        public int Points { get; set; } = 0;
        
        // Thêm trường UserId để liên kết với ApplicationUser
        public string? UserId { get; set; }
        
        // Thuộc tính điều hướng đến ApplicationUser
        public ApplicationUser? User { get; set; }
        
        // Navigation properties
        public ICollection<Order> Orders { get; set; } = new List<Order>();
        
        // Thêm thuộc tính điều hướng Reservations
        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    }
} 