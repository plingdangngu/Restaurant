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
    public class InvoiceRepository : IInvoiceRepository
    {
        private readonly ApplicationDbContext _context;

        public InvoiceRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Invoice>> GetAllAsync()
        {
            return await _context.Invoices
                .Include(i => i.Order)
                    .ThenInclude(o => o.Table)
                .Include(i => i.Order)
                    .ThenInclude(o => o.Customer)
                .OrderByDescending(i => i.InvoiceDate)
                .ToListAsync();
        }

        public async Task<Invoice> GetByIdAsync(int id)
        {
            return await _context.Invoices.FindAsync(id);
        }

        public async Task<Invoice> GetByIdWithDetailsAsync(int id)
        {
            return await _context.Invoices
                .Include(i => i.Order)
                    .ThenInclude(o => o.Table)
                .Include(i => i.Order)
                    .ThenInclude(o => o.Customer)
                .Include(i => i.Order)
                    .ThenInclude(o => o.OrderItems)
                        .ThenInclude(oi => oi.MenuItem)
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Invoices.AnyAsync(i => i.Id == id);
        }

        public async Task AddAsync(Invoice invoice)
        {
            await _context.Invoices.AddAsync(invoice);
        }

        public void Update(Invoice invoice)
        {
            _context.Invoices.Update(invoice);
        }

        public void Remove(Invoice invoice)
        {
            _context.Invoices.Remove(invoice);
        }

        public async Task<List<Invoice>> GetByCustomerIdAsync(string customerId)
        {
            return await _context.Invoices
                .Include(i => i.Order)
                    .ThenInclude(o => o.Table)
                .Where(i => i.Order.CustomerId == customerId)
                .OrderByDescending(i => i.InvoiceDate)
                .ToListAsync();
        }

        public async Task<List<Invoice>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Invoices
                .Include(i => i.Order)
                    .ThenInclude(o => o.Table)
                .Include(i => i.Order)
                    .ThenInclude(o => o.Customer)
                .Where(i => i.InvoiceDate.Date >= startDate.Date && i.InvoiceDate.Date <= endDate.Date)
                .OrderByDescending(i => i.InvoiceDate)
                .ToListAsync();
        }

        public async Task<List<Invoice>> GetByStatusAsync(string status)
        {
            return await _context.Invoices
                .Include(i => i.Order)
                    .ThenInclude(o => o.Table)
                .Include(i => i.Order)
                    .ThenInclude(o => o.Customer)
                .Where(i => i.Status == status)
                .OrderByDescending(i => i.InvoiceDate)
                .ToListAsync();
        }

        public async Task<decimal> GetTotalRevenueAsync(DateTime startDate, DateTime endDate)
        {
            var invoices = await _context.Invoices
                .Where(i => i.InvoiceDate.Date >= startDate.Date && 
                            i.InvoiceDate.Date <= endDate.Date && 
                            i.Status == "Paid")
                .ToListAsync();

            return invoices.Sum(i => i.TotalAmount);
        }

        public async Task<List<InvoiceReportViewModel>> GetInvoiceReportDataAsync(DateTime startDate, DateTime endDate)
        {
            var invoices = await _context.Invoices
                .Include(i => i.Order)
                    .ThenInclude(o => o.Table)
                .Include(i => i.Order)
                    .ThenInclude(o => o.Customer)
                .Include(i => i.Order)
                    .ThenInclude(o => o.Employee)
                .Where(i => i.InvoiceDate.Date >= startDate.Date && i.InvoiceDate.Date <= endDate.Date && i.Status == "Paid")
                .OrderByDescending(i => i.InvoiceDate)
                .ToListAsync();

            return invoices.Select(i => new InvoiceReportViewModel
            {
                InvoiceId = i.Id,
                InvoiceDate = i.InvoiceDate,
                CustomerName = i.Order?.Customer?.Name ?? "Walk-in Customer",
                EmployeeName = i.Order?.Employee?.FullName ?? "System",
                TableName = i.Order?.Table?.Name ?? "N/A",
                SubTotal = i.SubTotal,
                Tax = i.TaxAmount,
                Discount = i.DiscountAmount,
                TotalAmount = i.TotalAmount,
                PaymentMethod = i.PaymentMethod
            }).ToList();
        }

        public async Task<Order> GetOrderByIdAsync(int orderId)
        {
            return await _context.Orders
                .Include(o => o.Table)
                .Include(o => o.Customer)
                .Include(o => o.Employee)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.MenuItem)
                .FirstOrDefaultAsync(o => o.Id == orderId);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
} 