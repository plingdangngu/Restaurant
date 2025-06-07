using System.ComponentModel.DataAnnotations;

namespace RestaurantManagement.ViewModels
{
    public class CartItemViewModel
    {
        public int MenuItemId { get; set; }
        public string MenuItemName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string Notes { get; set; }
        public decimal Subtotal => Price * Quantity;

        public CartItemViewModel() { }

        public CartItemViewModel(int menuItemId, string menuItemName, decimal price, int quantity, string notes)
        {
            MenuItemId = menuItemId;
            MenuItemName = menuItemName;
            Price = price;
            Quantity = quantity;
            Notes = notes;
        }
    }
} 