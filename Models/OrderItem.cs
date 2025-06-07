using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestaurantManagement.Models
{
    public class OrderItem
    {
        public int Id { get; set; }
        
        [Required]
        public int OrderId { get; set; }
        
        [Required]
        public int MenuItemId { get; set; }
        
        [Required]
        [Range(1, 100, ErrorMessage = "Số lượng phải từ 1 đến 100")]
        public int Quantity { get; set; } = 1;
        
        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Price { get; set; }
        
        [NotMapped]
        public decimal UnitPrice { get => Price; set => Price = value; }
        
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Subtotal { get; set; }
        
        public string Status { get; set; } = "Pending"; // Pending, Preparing, Ready, Served, Cancelled
        
        [StringLength(500)]
        public string? SpecialRequests { get; set; }
        
        // Navigation properties
        public Order? Order { get; set; }
        
        public MenuItem? MenuItem { get; set; }
    }
} 