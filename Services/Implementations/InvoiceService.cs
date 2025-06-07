using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Data;
using RestaurantManagement.Models;
using RestaurantManagement.Services.Interfaces;
using RestaurantManagement.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantManagement.Services.Implementations
{
    public class InvoiceService : IInvoiceService
    {
        private readonly ApplicationDbContext _context;

        public InvoiceService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Invoice>> GetAllInvoicesAsync()
        {
            return await _context.Invoices
                .Include(i => i.Order)
                    .ThenInclude(o => o.Table)
                .OrderByDescending(i => i.PaymentDate)
                .ToListAsync();
        }

        public async Task<Invoice> GetInvoiceByIdAsync(int id)
        {
            return await _context.Invoices
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<Invoice> GetInvoiceWithDetailsAsync(int id)
        {
            return await _context.Invoices
                .Include(i => i.Order)
                    .ThenInclude(o => o.Table)
                .Include(i => i.Order)
                    .ThenInclude(o => o.User)
                .Include(i => i.Order)
                    .ThenInclude(o => o.OrderItems)
                        .ThenInclude(oi => oi.MenuItem)
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<Invoice> GetInvoiceForDeleteAsync(int id)
        {
            return await _context.Invoices
                .Include(i => i.Order)
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<(bool Success, string Message, Invoice Invoice)> CreateInvoiceAsync(int orderId)
        {
            try
            {
                var order = await _context.Orders
                    .Include(o => o.OrderItems)
                    .FirstOrDefaultAsync(o => o.Id == orderId);

                if (order == null)
                    return (false, "Không tìm thấy đơn hàng", null);

                if (order.Status == "Completed")
                    return (false, "Đơn hàng này đã được thanh toán", null);

                // Tính tổng tiền
                decimal totalAmount = order.OrderItems.Sum(oi => oi.Subtotal);

                // Tạo hóa đơn
                var invoice = new Invoice
                {
                    OrderId = orderId,
                    PaymentDate = DateTime.Now,
                    PaymentMethod = "Cash",
                    Amount = totalAmount,
                    Status = "Paid"
                };

                _context.Invoices.Add(invoice);

                // Cập nhật trạng thái đơn hàng
                order.Status = "Completed";
                _context.Update(order);

                await _context.SaveChangesAsync();

                return (true, "Tạo hóa đơn thành công!", invoice);
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi: {ex.Message}", null);
            }
        }

        public async Task<(bool Success, string Message)> UpdateInvoiceAsync(Invoice invoice)
        {
            try
            {
                _context.Update(invoice);
                await _context.SaveChangesAsync();
                return (true, "Cập nhật hóa đơn thành công!");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await InvoiceExistsAsync(invoice.Id))
                    return (false, "Không tìm thấy hóa đơn");
                else
                    throw;
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi: {ex.Message}");
            }
        }

        public async Task<(bool Success, string Message)> DeleteInvoiceAsync(int id)
        {
            try
            {
                var invoice = await _context.Invoices.FindAsync(id);
                if (invoice == null)
                    return (false, "Không tìm thấy hóa đơn");

                _context.Invoices.Remove(invoice);
                await _context.SaveChangesAsync();
                return (true, "Xóa hóa đơn thành công!");
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi: {ex.Message}");
            }
        }

        public async Task<bool> InvoiceExistsAsync(int id)
        {
            return await _context.Invoices.AnyAsync(i => i.Id == id);
        }

        public async Task<List<Invoice>> GetInvoicesByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Invoices
                .Include(i => i.Order)
                    .ThenInclude(o => o.Table)
                .Include(i => i.Order)
                    .ThenInclude(o => o.User)
                .Where(i => i.PaymentDate.Date >= startDate.Date && i.PaymentDate.Date <= endDate.Date)
                .OrderByDescending(i => i.PaymentDate)
                .ToListAsync();
        }

        public async Task<List<Invoice>> GetInvoicesByCustomerIdAsync(string customerId)
        {
            if (string.IsNullOrEmpty(customerId))
                return new List<Invoice>();

            return await _context.Invoices
                .Include(i => i.Order)
                    .ThenInclude(o => o.Table)
                .Include(i => i.Order)
                    .ThenInclude(o => o.User)
                .Where(i => i.Order.CustomerId == customerId)
                .OrderByDescending(i => i.PaymentDate)
                .ToListAsync();
        }

        public async Task<decimal> GetTotalRevenueAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _context.Invoices.AsQueryable();

            if (startDate.HasValue)
                query = query.Where(i => i.PaymentDate.Date >= startDate.Value.Date);

            if (endDate.HasValue)
                query = query.Where(i => i.PaymentDate.Date <= endDate.Value.Date);

            return await query.SumAsync(i => i.TotalAmount);
        }

        public async Task<Dictionary<string, decimal>> GetRevenueByPeriodAsync(string period, int count)
        {
            var result = new Dictionary<string, decimal>();
            var today = DateTime.Today;
            
            switch (period.ToLower())
            {
                case "day":
                    // Revenue for the last 'count' days
                    for (int i = count - 1; i >= 0; i--)
                    {
                        var date = today.AddDays(-i);
                        var revenue = await GetTotalSalesByDateAsync(date);
                        result.Add(date.ToString("dd/MM"), revenue);
                    }
                    break;
                    
                case "month":
                    // Revenue for the last 'count' months
                    for (int i = count - 1; i >= 0; i--)
                    {
                        var date = today.AddMonths(-i);
                        var firstDay = new DateTime(date.Year, date.Month, 1);
                        var lastDay = firstDay.AddMonths(1).AddDays(-1);
                        
                        var revenue = await GetTotalSalesByDateRangeAsync(firstDay, lastDay);
                        result.Add(date.ToString("MM/yyyy"), revenue);
                    }
                    break;
                    
                case "year":
                    // Revenue for the last 'count' years
                    for (int i = count - 1; i >= 0; i--)
                    {
                        var date = today.AddYears(-i);
                        var firstDay = new DateTime(date.Year, 1, 1);
                        var lastDay = new DateTime(date.Year, 12, 31);
                        
                        var revenue = await GetTotalSalesByDateRangeAsync(firstDay, lastDay);
                        result.Add(date.Year.ToString(), revenue);
                    }
                    break;
                    
                default:
                    // Default to daily for last 7 days
                    for (int i = 6; i >= 0; i--)
                    {
                        var date = today.AddDays(-i);
                        var revenue = await GetTotalSalesByDateAsync(date);
                        result.Add(date.ToString("dd/MM"), revenue);
                    }
                    break;
            }
            
            return result;
        }

        public async Task<InvoiceViewModel> PrepareInvoiceViewModelAsync(int id)
        {
            var invoice = await GetInvoiceWithDetailsAsync(id);
            if (invoice == null)
                return null;

            var order = invoice.Order;
            var customer = order.CustomerId != null && !string.IsNullOrEmpty(order.CustomerId) ? 
                await _context.Customers.FindAsync(order.CustomerId) : null;

            var viewModel = new InvoiceViewModel
            {
                Id = invoice.Id,
                InvoiceNumber = $"INV-{invoice.Id:D6}",
                OrderId = invoice.OrderId,
                CustomerName = customer?.Name ?? "Khách lẻ",
                TableName = order.Table?.Name ?? "N/A",
                CreatedDate = invoice.PaymentDate,
                TotalAmount = invoice.Amount,
                Status = invoice.Status,
                Notes = invoice.Notes,
                CreatedBy = order.User?.UserName ?? "System",
                Items = order.OrderItems.ToList()
            };

            return viewModel;
        }

        public async Task<decimal> GetTotalSalesByDateAsync(DateTime date)
        {
            return await _context.Invoices
                .Where(i => i.PaymentDate.Date == date.Date)
                .SumAsync(i => i.TotalAmount);
        }

        public async Task<decimal> GetTotalSalesByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Invoices
                .Where(i => i.PaymentDate.Date >= startDate.Date && i.PaymentDate.Date <= endDate.Date)
                .SumAsync(i => i.TotalAmount);
        }

        public async Task<List<InvoiceReportViewModel>> GetInvoiceReportAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _context.Invoices
                .Include(i => i.Order)
                    .ThenInclude(o => o.Table)
                .Include(i => i.Order)
                    .ThenInclude(o => o.User)
                .AsQueryable();

            // Lọc theo khoảng thời gian nếu có
            if (startDate.HasValue)
                query = query.Where(i => i.PaymentDate.Date >= startDate.Value.Date);

            if (endDate.HasValue)
                query = query.Where(i => i.PaymentDate.Date <= endDate.Value.Date);

            var invoices = await query
                .OrderByDescending(i => i.PaymentDate)
                .ToListAsync();

            var report = invoices.Select(i => new InvoiceReportViewModel
            {
                Id = i.Id,
                InvoiceNumber = $"INV-{i.Id:D6}",
                OrderId = i.OrderId,
                CustomerName = i.Order?.Customer?.Name ?? "Khách lẻ",
                TableName = i.Order?.Table?.Name ?? "N/A",
                CreatedDate = i.PaymentDate,
                TotalAmount = i.Amount,
                Tax = i.TaxAmount,
                GrandTotal = i.TotalAmount,
                Status = i.Status,
                CreatedBy = i.Order?.User?.UserName ?? "System"
            }).ToList();

            return report;
        }
    }
} 