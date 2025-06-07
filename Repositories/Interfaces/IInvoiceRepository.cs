using RestaurantManagement.Models;
using RestaurantManagement.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RestaurantManagement.Repositories.Interfaces
{
    public interface IInvoiceRepository
    {
        Task<List<Invoice>> GetAllAsync();
        Task<Invoice> GetByIdAsync(int id);
        Task<Invoice> GetByIdWithDetailsAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task AddAsync(Invoice invoice);
        void Update(Invoice invoice);
        void Remove(Invoice invoice);
        Task<List<Invoice>> GetByCustomerIdAsync(string customerId);
        Task<List<Invoice>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<List<Invoice>> GetByStatusAsync(string status);
        Task<decimal> GetTotalRevenueAsync(DateTime startDate, DateTime endDate);
        Task<List<InvoiceReportViewModel>> GetInvoiceReportDataAsync(DateTime startDate, DateTime endDate);
        Task<Order> GetOrderByIdAsync(int orderId);
        Task SaveChangesAsync();
    }
} 