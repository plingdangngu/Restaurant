using System.ComponentModel.DataAnnotations;
using RestaurantManagement.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace RestaurantManagement.ViewModels
{
    public class OrderViewModel
    {
        public int Id { get; set; }
        public int TableId { get; set; }
        public string? TableName { get; set; }
        public string? CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public string? EmployeeName { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public decimal Total { get; set; }
        public decimal TotalAmount { get; set; }
        public int ItemCount { get; set; }
        public List<OrderItemViewModel> Items { get; set; } = new();
        public List<Table> AvailableTables { get; set; } = new();
    }
    
    public class OrderItemViewModel
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int MenuItemId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Subtotal => Quantity * UnitPrice;
        public string? SpecialRequests { get; set; }
    }
        
    public class MenuItemOption
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Category { get; set; } = string.Empty;
    }
} 