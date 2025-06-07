using RestaurantManagement.Models;
using RestaurantManagement.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RestaurantManagement.Repositories.Interfaces
{
    public interface IDashboardRepository
    {
        Task<int> GetTotalTableCountAsync();
        Task<int> GetOccupiedTableCountAsync();
        Task<int> GetAvailableTableCountAsync();
        Task<int> GetPendingOrdersCountAsync();
        Task<int> GetTodayReservationsCountAsync();
        Task<int> GetTodayOrdersCountAsync();
        Task<decimal> GetTodayRevenueAsync();
        Task<decimal> GetMonthlyRevenueAsync(DateTime date);
        Task<List<DashboardOrderViewModel>> GetRecentOrdersAsync(int count = 5);
        Task<List<Reservation>> GetUpcomingReservationsAsync(int count = 5);
        Task<Dictionary<string, decimal>> GetRevenueTrendAsync(DateTime startDate, DateTime endDate);
        Task<Dictionary<string, int>> GetPopularMenuItemsAsync(int count = 5);
    }
} 