using System;

namespace RestaurantManagement.ViewModels
{
    public class DashboardOrderViewModel
    {
        public int Id { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public string? CustomerName { get; set; }
        public string? TableName { get; set; }
        public decimal Total { get; set; }
    }

    public class DashboardReservationViewModel
    {
        public int Id { get; set; }
        public DateTime ReservationDate { get; set; }
        public TimeSpan ReservationTime { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string? CustomerPhone { get; set; }
        public string? TableName { get; set; }
        public int PartySize { get; set; }
        public string Status { get; set; } = string.Empty;
    }
} 