using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestaurantManagement.Models
{
    public class MenuItem
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Vui lòng nhập tên món")]
        [StringLength(200, ErrorMessage = "Tên món không được vượt quá 200 ký tự")]
        public string Name { get; set; } = string.Empty;
        
        [StringLength(500, ErrorMessage = "Mô tả không được vượt quá 500 ký tự")]
        public string? Description { get; set; }
        
        [Required(ErrorMessage = "Vui lòng nhập giá món")]
        [Range(0, 10000000, ErrorMessage = "Giá phải từ 0 đến 10,000,000")]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Price { get; set; }
        
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? DiscountedPrice { get; set; }
        
        public string? ImageUrl { get; set; }
        
        public bool IsAvailable { get; set; } = true;
        
        public bool IsFeatured { get; set; } = false;
        
        public int EstimatedCookingTime { get; set; } = 15; // Thời gian nấu (phút)
        
        // Thêm thứ tự hiển thị
        public int DisplayOrder { get; set; } = 0;
        
        // Foreign keys
        [Required(ErrorMessage = "Vui lòng chọn danh mục")]
        public int MenuCategoryId { get; set; }
        
        // Thêm alias cho MenuCategoryId
        public int CategoryId { get => MenuCategoryId; set => MenuCategoryId = value; }
        
        // Navigation properties
        public MenuCategory? MenuCategory { get; set; }
        
        // Thêm property alias để tránh lỗi
        public MenuCategory? Category { get => MenuCategory; set => MenuCategory = value; }
        
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
} 