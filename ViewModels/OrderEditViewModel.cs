using Microsoft.AspNetCore.Mvc.Rendering;
using RestaurantManagement.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RestaurantManagement.ViewModels
{
    public class OrderEditViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn bàn")]
        [Display(Name = "Bàn")]
        public int TableId { get; set; }

        [Display(Name = "Khách hàng")]
        public int? CustomerId { get; set; }

        [Display(Name = "Trạng thái")]
        public string Status { get; set; }

        [Display(Name = "Ghi chú")]
        public string Notes { get; set; }

        [Display(Name = "Số người")]
        public int? NumberOfGuests { get; set; }

        [Display(Name = "Ngày đặt")]
        [DataType(DataType.DateTime)]
        public DateTime OrderDate { get; set; }

        [Display(Name = "Tổng tiền")]
        [DisplayFormat(DataFormatString = "{0:N0} đ")]
        public decimal TotalAmount { get; set; }

        // Items in the order
        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

        // Dropdown lists
        public SelectList Tables { get; set; }
        public SelectList Customers { get; set; }
        public SelectList StatusOptions { get; set; }
    }
} 