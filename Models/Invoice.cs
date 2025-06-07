using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestaurantManagement.Models
{
    public class Invoice
    {
        public int Id { get; set; }
        
        [Required]
        public int OrderId { get; set; }
        
        [Required]
        public string EmployeeId { get; set; } = string.Empty;
        
        [Required]
        public DateTime InvoiceDate { get; set; } = DateTime.Now;
        
        [NotMapped]
        public DateTime PaymentDate { get => InvoiceDate; set => InvoiceDate = value; }
        
        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal SubTotal { get; set; }
        
        [Column(TypeName = "decimal(18, 2)")]
        public decimal TaxAmount { get; set; }
        
        [NotMapped]
        public decimal Tax { get => TaxAmount; set => TaxAmount = value; }
        
        [Column(TypeName = "decimal(18, 2)")]
        public decimal DiscountAmount { get; set; }
        
        [NotMapped]
        public decimal Discount { get => DiscountAmount; set => DiscountAmount = value; }
        
        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalAmount { get; set; }
        
        [NotMapped]
        public decimal Amount { get => TotalAmount; set => TotalAmount = value; }
        
        [Column(TypeName = "decimal(18, 2)")]
        public decimal PaidAmount { get; set; }
        
        public string PaymentMethod { get; set; } = "Cash"; // Cash, Card, Banking
        
        public bool IsPaid { get; set; } = false;
        
        [StringLength(100)]
        public string Status { get; set; } = "Pending"; // Pending, Paid, Cancelled
        
        [StringLength(500)]
        public string? Notes { get; set; }
        
        // Navigation properties
        public Order? Order { get; set; }
        
        public ApplicationUser? Employee { get; set; }
    }
} 