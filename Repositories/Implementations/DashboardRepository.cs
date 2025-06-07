using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Data;
using RestaurantManagement.Models;
using RestaurantManagement.Repositories.Interfaces;
using RestaurantManagement.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantManagement.Repositories.Implementations
{
    public class DashboardRepository : IDashboardRepository
    {
        private readonly ApplicationDbContext _context;

        public DashboardRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> GetTotalTableCountAsync()
        {
            return await _context.Tables.CountAsync();
        }

        public async Task<int> GetOccupiedTableCountAsync()
        {
            return await _context.Tables.CountAsync(t => t.IsOccupied);
        }

        public async Task<int> GetAvailableTableCountAsync()
        {
            return await _context.Tables.CountAsync(t => !t.IsOccupied);
        }

        public async Task<int> GetPendingOrdersCountAsync()
        {
            return await _context.Orders.CountAsync(o => o.Status == "Pending" || o.Status == "Processing");
        }

        public async Task<int> GetTodayReservationsCountAsync()
        {
            var today = DateTime.Today;
            return await _context.Reservations
                .CountAsync(r => r.ReservationDate.Date == today && r.Status != "Canceled");
        }

        public async Task<int> GetTodayOrdersCountAsync()
        {
            var today = DateTime.Today;
            return await _context.Orders
                .CountAsync(o => o.OrderDate.Date == today);
        }

        public async Task<decimal> GetTodayRevenueAsync()
        {
            var today = DateTime.Today;
            var invoices = await _context.Invoices
                .Where(i => i.InvoiceDate.Date == today && i.Status == "Paid")
                .ToListAsync();

            return invoices.Sum(i => i.TotalAmount);
        }

        public async Task<decimal> GetMonthlyRevenueAsync(DateTime date)
        {
            var firstDayOfMonth = new DateTime(date.Year, date.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

            var invoices = await _context.Invoices
                .Where(i => i.InvoiceDate.Date >= firstDayOfMonth && 
                            i.InvoiceDate.Date <= lastDayOfMonth && 
                            i.Status == "Paid")
                .ToListAsync();

            return invoices.Sum(i => i.TotalAmount);
        }

        public async Task<List<DashboardOrderViewModel>> GetRecentOrdersAsync(int count = 5)
        {
            var orders = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Table)
                .Include(o => o.OrderItems)
                .OrderByDescending(o => o.OrderDate)
                .Take(count)
                .ToListAsync();

            return orders.Select(o => new DashboardOrderViewModel
            {
                Id = o.Id,
                CustomerName = o.Customer?.FullName ?? "Khách vãng lai",
                TableName = o.Table?.Name ?? "N/A",
                OrderDate = o.OrderDate,
                Total = o.OrderItems.Sum(oi => oi.Quantity * oi.UnitPrice),
                Status = o.Status
            }).ToList();
        }

        public async Task<List<Reservation>> GetUpcomingReservationsAsync(int count = 5)
        {
            var now = DateTime.Now;
            return await _context.Reservations
                .Include(r => r.Customer)
                .Include(r => r.Table)
                .Where(r => (r.ReservationDate > now.Date || 
                           (r.ReservationDate == now.Date && r.ReservationTime > now.TimeOfDay)) && 
                           r.Status == "Confirmed")
                .OrderBy(r => r.ReservationDate)
                .ThenBy(r => r.ReservationTime)
                .Take(count)
                .ToListAsync();
        }

        public async Task<Dictionary<string, decimal>> GetRevenueTrendAsync(DateTime startDate, DateTime endDate)
        {
            var invoices = await _context.Invoices
                .Where(i => i.InvoiceDate.Date >= startDate.Date && 
                            i.InvoiceDate.Date <= endDate.Date && 
                            i.Status == "Paid")
                .ToListAsync();

            return invoices
                .GroupBy(i => i.InvoiceDate.Date.ToString("yyyy-MM-dd"))
                .OrderBy(g => g.Key)
                .ToDictionary(g => g.Key, g => g.Sum(i => i.TotalAmount));
        }

        public async Task<Dictionary<string, int>> GetPopularMenuItemsAsync(int count = 5)
        {
            var orderItems = await _context.OrderItems
                .Include(oi => oi.MenuItem)
                .ToListAsync();

            return orderItems
                .GroupBy(oi => oi.MenuItemId + "-" + oi.MenuItem.Name)
                .OrderByDescending(g => g.Sum(oi => oi.Quantity))
                .Take(count)
                .ToDictionary(g => g.Key, g => g.Sum(oi => oi.Quantity));
        }
    }
} 