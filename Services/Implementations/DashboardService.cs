using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Models;
using RestaurantManagement.Models.Dashboard;
using RestaurantManagement.Repositories.Interfaces;
using RestaurantManagement.Services.Interfaces;
using RestaurantManagement.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantManagement.Services.Implementations
{
    public class DashboardService : IDashboardService
    {
        private readonly IDashboardRepository _dashboardRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IInvoiceRepository _invoiceRepository;

        public DashboardService(
            IDashboardRepository dashboardRepository,
            IOrderRepository orderRepository,
            IInvoiceRepository invoiceRepository)
        {
            _dashboardRepository = dashboardRepository;
            _orderRepository = orderRepository;
            _invoiceRepository = invoiceRepository;
        }

        public async Task<DashboardData> GetDashboardDataAsync()
        {
            // Get today's date
            var today = DateTime.Today;
            var now = DateTime.Now;
            
            // Get start of current week, month, and year
            var startOfWeek = today.AddDays(-(int)today.DayOfWeek + (int)DayOfWeek.Monday);
            var startOfMonth = new DateTime(today.Year, today.Month, 1);
            var startOfYear = new DateTime(today.Year, 1, 1);
            
            // 1. Revenue statistics
            var todayRevenue = await _dashboardRepository.GetTodayRevenueAsync();
            var weekRevenue = await CalculateRevenueForDateRangeAsync(startOfWeek, today);
            var monthRevenue = await _dashboardRepository.GetMonthlyRevenueAsync(today);
            var yearRevenue = await CalculateRevenueForDateRangeAsync(startOfYear, today);
                
            // 2. Order statistics
            var todayOrderCount = await _dashboardRepository.GetTodayOrdersCountAsync();
            var pendingOrderCount = await GetOrderCountByStatusAsync("Pending");
            var processingOrderCount = await GetOrderCountByStatusAsync("Processing");
            var completedOrderCount = await GetOrderCountByStatusAsync("Completed", today);
            var cancelledOrderCount = await GetOrderCountByStatusAsync("Cancelled", today);
                
            // 3. Table statistics
            var totalTableCount = await _dashboardRepository.GetTotalTableCountAsync();
            var occupiedTableCount = await _dashboardRepository.GetOccupiedTableCountAsync();
            var availableTableCount = await _dashboardRepository.GetAvailableTableCountAsync();
                
            // 4. Menu statistics
            var topMenuItems = await GetTopMenuItemsAsync(startOfMonth, now);
                
            // 5. Recent orders
            var recentOrdersViewModel = await _dashboardRepository.GetRecentOrdersAsync(10);
                
            // 6. Revenue chart data (last 7 days)
            var revenueChartData = await GetLast7DaysRevenueChartDataAsync(today);
            
            // Prepare dashboard view model
            return new DashboardData
            {
                TodayRevenue = todayRevenue,
                WeekRevenue = weekRevenue,
                MonthRevenue = monthRevenue,
                YearRevenue = yearRevenue,
                
                TodayOrderCount = todayOrderCount,
                PendingOrderCount = pendingOrderCount,
                ProcessingOrderCount = processingOrderCount,
                CompletedOrderCount = completedOrderCount,
                CancelledOrderCount = cancelledOrderCount,
                
                TotalTableCount = totalTableCount,
                OccupiedTableCount = occupiedTableCount,
                AvailableTableCount = availableTableCount,
                
                TopMenuItems = topMenuItems,
                RecentOrders = recentOrdersViewModel.Select(r => new Order
                {
                    Id = r.Id,
                    Status = r.Status,
                    OrderTime = r.OrderDate,
                    Customer = r.CustomerName != null ? new Customer { FullName = r.CustomerName } : null,
                    Table = r.TableName != null ? new Table { Name = r.TableName } : null
                }).ToList(),
                RevenueChartData = revenueChartData
            };
        }

        private async Task<decimal> CalculateRevenueForDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var revenueTrend = await _dashboardRepository.GetRevenueTrendAsync(startDate, endDate);
            return revenueTrend.Values.Sum();
        }

        private async Task<int> GetOrderCountByStatusAsync(string status, DateTime? date = null)
        {
            var orders = await _orderRepository.GetOrdersByStatusAsync(status);
            
            if (date.HasValue)
            {
                orders = orders.Where(o => o.OrderDate.Date == date.Value.Date).ToList();
            }
            
            return orders.Count;
        }

        private async Task<List<TopMenuItem>> GetTopMenuItemsAsync(DateTime startDate, DateTime endDate)
        {
            var popularItems = await _dashboardRepository.GetPopularMenuItemsAsync(5);
            
            var result = new List<TopMenuItem>();
            foreach (var item in popularItems)
            {
                var menuItem = await _orderRepository.GetMenuItemByIdAsync(int.Parse(item.Key.Split('-')[0]));
                if (menuItem != null)
                {
                    result.Add(new TopMenuItem
                    {
                        MenuItemId = menuItem.Id,
                        Name = menuItem.Name,
                        ImageUrl = menuItem.ImageUrl,
                        OrderCount = item.Value,
                        TotalRevenue = item.Value * menuItem.Price
                    });
                }
            }
            
            return result;
        }

        private async Task<List<DailyRevenue>> GetLast7DaysRevenueChartDataAsync(DateTime today)
        {
            var last7Days = Enumerable.Range(0, 7)
                .Select(i => today.AddDays(-i))
                .Reverse()
                .ToList();
                
            var startDate = last7Days.First();
            var endDate = last7Days.Last();
                
            var revenueTrend = await _dashboardRepository.GetRevenueTrendAsync(startDate, endDate);
            
            var result = new List<DailyRevenue>();
            foreach (var date in last7Days)
            {
                var dateStr = date.ToString("yyyy-MM-dd");
                var revenue = revenueTrend.ContainsKey(dateStr) ? revenueTrend[dateStr] : 0;
                
                result.Add(new DailyRevenue
                {
                    Date = date.ToString("dd/MM"),
                    Revenue = revenue
                });
            }
            
            return result;
        }
    }
} 