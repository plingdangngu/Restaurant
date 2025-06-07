using System.ComponentModel.DataAnnotations;

namespace RestaurantManagement.ViewModels
{
    public class MoveTableViewModel
    {
        [Required]
        public int SourceTableId { get; set; }
        
        [Required(ErrorMessage = "Vui lòng chọn bàn đích")]
        public int TargetTableId { get; set; }
        
        public string SourceTableName { get; set; } = string.Empty;
        
        public string SourceFloorName { get; set; } = string.Empty;
        
        public string? Note { get; set; }
    }
} 