using Microsoft.AspNetCore.Mvc.Rendering;
using RestaurantManagement.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RestaurantManagement.ViewModels
{
    public class AddItemViewModel
    {
        public int OrderId { get; set; }
        
        [Required(ErrorMessage = "Vui lòng chọn món ăn")]
        [Display(Name = "Món ăn")]
        public int MenuItemId { get; set; }
        
        [Required(ErrorMessage = "Vui lòng nhập số lượng")]
        [Range(1, 100, ErrorMessage = "Số lượng phải từ 1 đến 100")]
        [Display(Name = "Số lượng")]
        public int Quantity { get; set; }
        
        [Display(Name = "Ghi chú đặc biệt")]
        public string Notes { get; set; }
        
        [Display(Name = "Yêu cầu đặc biệt")]
        public string SpecialRequests { get; set; }
        
        public SelectList MenuItems { get; set; }
        
        // Thêm danh sách món ăn theo danh mục
        public Dictionary<string, List<MenuItem>> MenuCategories { get; set; } = new Dictionary<string, List<MenuItem>>();
    }
} 