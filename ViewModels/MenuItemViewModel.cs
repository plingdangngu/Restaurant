using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace RestaurantManagement.ViewModels
{
    public class MenuItemViewModel
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Vui lòng nhập tên món")]
        [StringLength(200, ErrorMessage = "Tên món không được vượt quá 200 ký tự")]
        public string Name { get; set; } = string.Empty;
        
        [StringLength(500, ErrorMessage = "Mô tả không được vượt quá 500 ký tự")]
        public string? Description { get; set; }
        
        [Required(ErrorMessage = "Vui lòng nhập giá món")]
        [Range(0, 10000000, ErrorMessage = "Giá phải từ 0 đến 10,000,000")]
        [Display(Name = "Giá")]
        public decimal Price { get; set; }
        
        [Display(Name = "Giá khuyến mãi")]
        public decimal? DiscountedPrice { get; set; }
        
        [Display(Name = "Ảnh món ăn")]
        public IFormFile? ImageFile { get; set; }
        
        public string? ExistingImagePath { get; set; }
        
        [Display(Name = "Đang phục vụ")]
        public bool IsAvailable { get; set; } = true;
        
        [Display(Name = "Món nổi bật")]
        public bool IsFeatured { get; set; } = false;
        
        [Display(Name = "Thời gian nấu (phút)")]
        [Range(1, 120, ErrorMessage = "Thời gian nấu phải từ 1 đến 120 phút")]
        public int EstimatedCookingTime { get; set; } = 15;
        
        [Required(ErrorMessage = "Vui lòng chọn danh mục")]
        [Display(Name = "Danh mục")]
        public int MenuCategoryId { get; set; }
        
        // Thêm CategoryId - Alias của MenuCategoryId để tương thích với code
        public int CategoryId { get => MenuCategoryId; set => MenuCategoryId = value; }
        
        // Thêm DisplayOrder
        [Display(Name = "Thứ tự hiển thị")]
        public int DisplayOrder { get; set; } = 0;
        
        // Thêm ImageUrl 
        public string? ImageUrl { get; set; }
        
        // Thêm MenuCategories
        public SelectList? MenuCategories { get; set; }
        
        // Cờ để xác định liệu có trở về trang chi tiết danh mục hay không
        public bool ReturnToCategoryDetails { get; set; } = false;
    }
} 