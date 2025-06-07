using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RestaurantManagement.ViewModels
{
    public class OrderCreateViewModel
    {
        [Required(ErrorMessage = "Vui lòng chọn bàn")]
        [Display(Name = "Bàn")]
        public int TableId { get; set; }

        [Display(Name = "Khách hàng")]
        public string? CustomerId { get; set; }

        [Display(Name = "Ghi chú")]
        public string Notes { get; set; }

        [Display(Name = "Số người")]
        public int? NumberOfGuests { get; set; }

        [Display(Name = "Ngày đặt")]
        [DataType(DataType.Date)]
        public DateTime OrderDate { get; set; } = DateTime.Now;

        // Dropdown lists
        public SelectList Tables { get; set; }
        public SelectList Customers { get; set; }
        
        // Thêm thuộc tính TablesSelectList
        public SelectList TablesSelectList { get; set; }
    }
} 