using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace RestaurantManagement.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;
        
        [StringLength(200)]
        public string? Address { get; set; }
        
        public DateTime? BirthDate { get; set; }
        
        public DateTime JoinDate { get; set; } = DateTime.Now;
        
        public bool IsActive { get; set; } = true;
        
        [StringLength(500)]
        public string? Notes { get; set; }
        
        // Navigation Properties
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
} 