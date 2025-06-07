using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RestaurantManagement.ViewModels
{
    public class MultipleItemsViewModel
    {
        public int OrderId { get; set; }
        public List<CartItemViewModel> CartItems { get; set; } = new List<CartItemViewModel>();

        // Tổng giá trị giỏ hàng
        public decimal CartTotal => CartItems?.Sum(item => item.Subtotal) ?? 0;
        
        // Số lượng món trong giỏ hàng
        public int ItemCount => CartItems?.Count ?? 0;
    }
} 