using RestaurantManagement.Models;

namespace RestaurantManagement.ViewModels
{
    public class CustomerMenuViewModel
    {
        public List<MenuCategory> Categories { get; set; } = new List<MenuCategory>();
        public List<MenuItem> MenuItems { get; set; } = new List<MenuItem>();
        public int? SelectedCategoryId { get; set; }
    }
} 