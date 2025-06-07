using System.ComponentModel.DataAnnotations;

namespace RestaurantManagement.Models
{
    public class MenuCategory
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Vui lòng nhập tên danh mục")]
        [StringLength(100, ErrorMessage = "Tên danh mục không được vượt quá 100 ký tự")]
        public string Name { get; set; } = string.Empty;
        
        [StringLength(500, ErrorMessage = "Mô tả không được vượt quá 500 ký tự")]
        public string? Description { get; set; }
        
        public int DisplayOrder { get; set; } = 0;
        
        public bool IsAvailable { get; set; } = true;

        // Navigation property
        public ICollection<MenuItem> MenuItems { get; set; } = new List<MenuItem>();
    }
} 