using System.ComponentModel.DataAnnotations;

namespace RestaurantManagement.ViewModels
{
    public class PaymentViewModel
    {
        public int OrderId { get; set; }
        
        [Required(ErrorMessage = "Vui lòng chọn phương thức thanh toán")]
        [Display(Name = "Phương thức thanh toán")]
        public string PaymentMethod { get; set; }
        
        [Display(Name = "Tổng tiền")]
        [DisplayFormat(DataFormatString = "{0:N0} đ")]
        public decimal TotalAmount { get; set; }
        
        [Display(Name = "Giảm giá")]
        [DisplayFormat(DataFormatString = "{0:N0} đ")]
        public decimal DiscountAmount { get; set; }
        
        [Display(Name = "Thuế VAT (10%)")]
        [DisplayFormat(DataFormatString = "{0:N0} đ")]
        public decimal Tax => TotalAmount * 0.1m;
        
        [Display(Name = "Thành tiền")]
        [DisplayFormat(DataFormatString = "{0:N0} đ")]
        public decimal GrandTotal => TotalAmount + Tax - DiscountAmount;
        
        [Display(Name = "Tiền khách trả")]
        public decimal AmountPaid { get; set; }
        
        [Display(Name = "Tiền thối lại")]
        [DisplayFormat(DataFormatString = "{0:N0} đ")]
        public decimal Change => AmountPaid - GrandTotal > 0 ? AmountPaid - GrandTotal : 0;
        
        [Display(Name = "Ghi chú")]
        public string Notes { get; set; }
    }
} 