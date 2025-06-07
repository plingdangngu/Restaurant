using System;
using System.ComponentModel.DataAnnotations;

namespace RestaurantManagement.ViewModels
{
    public class InvoiceReportViewModel
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
        
        [Display(Name = "Thuế VAT")]
        [DisplayFormat(DataFormatString = "{0:N0} đ")]
        public decimal Tax { get; set; }
        
        [Display(Name = "Thành tiền")]
        [DisplayFormat(DataFormatString = "{0:N0} đ")]
        public decimal GrandTotal { get; set; }
        
        [Display(Name = "Trạng thái")]
        public string Status { get; set; }
        
        [Display(Name = "Người tạo")]
        public string CreatedBy { get; set; }
        
        [Display(Name = "ID Hóa đơn")]
        public int InvoiceId { get; set; }
        
        [Display(Name = "Ngày hóa đơn")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}")]
        public DateTime InvoiceDate { get; set; }
        
        [Display(Name = "Nhân viên")]
        public string EmployeeName { get; set; }
        
        [Display(Name = "Tổng tiền hàng")]
        [DisplayFormat(DataFormatString = "{0:N0} đ")]
        public decimal SubTotal { get; set; }
        
        [Display(Name = "Giảm giá")]
        [DisplayFormat(DataFormatString = "{0:N0} đ")]
        public decimal Discount { get; set; }
        
        [Display(Name = "Phương thức thanh toán")]
        public string PaymentMethod { get; set; }
        
        [Display(Name = "Ngày thanh toán")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}")]
        public DateTime PaymentDate { get; set; }
        
        [Display(Name = "Tổng cộng")]
        [DisplayFormat(DataFormatString = "{0:N0} đ")]
        public decimal Total { get; set; }
        
        [Display(Name = "Người dùng")]
        public string UserName { get; set; }
    }
} 