using RestaurantManagement.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RestaurantManagement.ViewModels
{
    public class InvoiceViewModel
    {
        public int Id { get; set; }
        
        [Display(Name = "Mã hóa đơn")]
        public string InvoiceNumber { get; set; }
        
        [Display(Name = "Mã đơn hàng")]
        public int OrderId { get; set; }
        
        [Display(Name = "Khách hàng")]
        public string CustomerName { get; set; }
        
        [Display(Name = "Bàn")]
        public string TableName { get; set; }
        
        [Display(Name = "Ngày tạo")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}")]
        public DateTime CreatedDate { get; set; }
        
        [Display(Name = "Tổng tiền")]
        [DisplayFormat(DataFormatString = "{0:N0} đ")]
        public decimal TotalAmount { get; set; }
        
        [Display(Name = "Thuế VAT (10%)")]
        [DisplayFormat(DataFormatString = "{0:N0} đ")]
        public decimal Tax => TotalAmount * 0.1m;
        
        [Display(Name = "Thành tiền")]
        [DisplayFormat(DataFormatString = "{0:N0} đ")]
        public decimal GrandTotal => TotalAmount + Tax;
        
        [Display(Name = "Trạng thái")]
        public string Status { get; set; }
        
        [Display(Name = "Ghi chú")]
        public string Notes { get; set; }
        
        [Display(Name = "Người tạo")]
        public string CreatedBy { get; set; }
        
        // Chi tiết hóa đơn
        public List<OrderItem> Items { get; set; } = new List<OrderItem>();
    }
} 