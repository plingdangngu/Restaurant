using System.ComponentModel.DataAnnotations;

namespace RestaurantManagement.Models
{
    public class Reservation
    {
        public int Id { get; set; }
        
        [Required]
        public DateTime ReservationDate { get; set; }
        
        [Required]
        public TimeSpan ReservationTime { get; set; }
        
        [Required]
        [Range(1, 100, ErrorMessage = "Số người phải từ 1 đến 100")]
        public int PartySize { get; set; }
        
        public string Status { get; set; } = "Pending";
        
        [StringLength(500)]
        public string? Notes { get; set; }
        
        // Foreign keys
        public string? CustomerId { get; set; }
        public int? TableId { get; set; }
        
        // Navigation properties
        public Customer? Customer { get; set; }
        public Table? Table { get; set; }
    }
} 