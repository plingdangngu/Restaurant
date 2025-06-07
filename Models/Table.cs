namespace RestaurantManagement.Models
{
    public class Table
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public bool IsOccupied { get; set; }
        public int FloorId { get; set; }
        public Floor? Floor { get; set; }
        public string? Status { get; set; } // Available, Reserved, Occupied, Cleaning
        
        // Navigation properties
        public List<Order> Orders { get; set; } = new List<Order>();
        public List<Invoice> Invoices { get; set; } = new List<Invoice>();
    }
} 