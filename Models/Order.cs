using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestaurantManagement.Models
{
    public class Order
    {
        public int Id { get; set; }
        
        [Required]
        public int TableId { get; set; }
        
        public string? CustomerId { get; set; }
        
        [Required]
        public string EmployeeId { get; set; } = string.Empty;
        
        [Required]
        public string UserId { get; set; } = string.Empty;
        
        [Required]
        public DateTime OrderTime { get; set; } = DateTime.Now;
        
        [Required]
        public DateTime OrderDate { get; set; } = DateTime.Now;
        
        public DateTime? CompletionTime { get; set; }
        
        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalAmount { get; set; }
        
        public string Status { get; set; } = "Pending"; // Pending, Processing, Completed, Cancelled
        
        public int? PeopleCount { get; set; }
        
        [StringLength(500)]
        public string? Notes { get; set; }
        
        // Navigation properties
        public Table? Table { get; set; }
        
        public Customer? Customer { get; set; }
        
        public ApplicationUser? Employee { get; set; }
        
        // Property alias cho Employee
        [NotMapped]
        public ApplicationUser? User { get => Employee; set => Employee = value; }
        
        public Invoice? Invoice { get; set; }
        
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
} 