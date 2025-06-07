using System;
using System.Collections.Generic;

namespace RestaurantManagement.Models.Dashboard
{
    public class DashboardData
    {
        // Revenue statistics
        public decimal TodayRevenue { get; set; }
        public decimal WeekRevenue { get; set; }
        public decimal MonthRevenue { get; set; }
        public decimal YearRevenue { get; set; }
        
        // Order statistics
        public int TodayOrderCount { get; set; }
        public int PendingOrderCount { get; set; }
        public int ProcessingOrderCount { get; set; }
        public int CompletedOrderCount { get; set; }
        public int CancelledOrderCount { get; set; }
        
        // Table statistics
        public int TotalTableCount { get; set; }
        public int OccupiedTableCount { get; set; }
        public int AvailableTableCount { get; set; }
        
        // Menu statistics
        public List<TopMenuItem> TopMenuItems { get; set; }
        
        // Recent orders
        public List<Order> RecentOrders { get; set; }
        
        // Chart data
        public List<DailyRevenue> RevenueChartData { get; set; }
    }
    
    public class TopMenuItem
    {
        public int MenuItemId { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public int OrderCount { get; set; }
        public decimal TotalRevenue { get; set; }
    }
    
    public class DailyRevenue
    {
        public string Date { get; set; }
        public decimal Revenue { get; set; }
    }
} 