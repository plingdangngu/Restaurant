using RestaurantManagement.Models;
using RestaurantManagement.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RestaurantManagement.Services.Interfaces
{
    public interface IInvoiceService
    {
        Task<List<Invoice>> GetAllInvoicesAsync();
        Task<Invoice> GetInvoiceByIdAsync(int id);
        Task<Invoice> GetInvoiceWithDetailsAsync(int id);
        Task<(bool Success, string Message, Invoice Invoice)> CreateInvoiceAsync(int orderId);
        Task<(bool Success, string Message)> UpdateInvoiceAsync(Invoice invoice);
        Task<Invoice> GetInvoiceForDeleteAsync(int id);
        Task<(bool Success, string Message)> DeleteInvoiceAsync(int id);
        Task<bool> InvoiceExistsAsync(int id);
        Task<List<Invoice>> GetInvoicesByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<List<Invoice>> GetInvoicesByCustomerIdAsync(string customerId);
        Task<decimal> GetTotalRevenueAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<Dictionary<string, decimal>> GetRevenueByPeriodAsync(string period, int count);
        Task<InvoiceViewModel> PrepareInvoiceViewModelAsync(int id);
        Task<decimal> GetTotalSalesByDateAsync(System.DateTime date);
        Task<decimal> GetTotalSalesByDateRangeAsync(System.DateTime startDate, System.DateTime endDate);
        Task<List<InvoiceReportViewModel>> GetInvoiceReportAsync(System.DateTime? startDate, System.DateTime? endDate);
    }
} 