namespace RestaurantManagement.Models
{
    public class Floor
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }
        
        // Navigation properties
        public List<Table> Tables { get; set; } = new List<Table>();
    }
} 