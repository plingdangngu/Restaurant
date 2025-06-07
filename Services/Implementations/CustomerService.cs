using Microsoft.AspNetCore.Mvc.Rendering;
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
    public class CustomerService : ICustomerService
    {
        private readonly ApplicationDbContext _context;

        public CustomerService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Customer>> GetAllCustomersAsync()
        {
            return await _context.Customers
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<Customer> GetCustomerByIdAsync(int id)
        {
            return await _context.Customers.FindAsync(id);
        }

        public async Task<CustomerViewModel> PrepareCreateViewModelAsync()
        {
            return new CustomerViewModel();
        }

        public async Task<CustomerViewModel> PrepareEditViewModelAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return null;

            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
                return null;

            return new CustomerViewModel
            {
                Id = customer.Id,
                Name = customer.Name,
                Phone = customer.Phone,
                Email = customer.Email,
                Address = customer.Address,
                Notes = customer.Notes
            };
        }

        public async Task<(bool Success, string Message, Customer Customer)> CreateCustomerAsync(Customer customer)
        {
            if (customer == null)
                return (false, "Khách hàng không hợp lệ", null);

            try
            {
                // Kiểm tra SĐT đã được sử dụng chưa
                if (!string.IsNullOrEmpty(customer.Phone))
                {
                    var existingCustomer = await _context.Customers
                        .FirstOrDefaultAsync(c => c.Phone == customer.Phone);
                    
                    if (existingCustomer != null)
                        return (false, "Số điện thoại này đã được sử dụng", null);
                }

                _context.Add(customer);
                await _context.SaveChangesAsync();
                return (true, "Thêm khách hàng thành công!", customer);
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi: {ex.Message}", null);
            }
        }

        public async Task<(bool Success, string Message)> UpdateCustomerAsync(Customer customer)
        {
            if (customer == null)
                return (false, "Khách hàng không hợp lệ");

            try
            {
                // Kiểm tra SĐT đã được sử dụng chưa
                if (!string.IsNullOrEmpty(customer.Phone))
                {
                    var existingCustomer = await _context.Customers
                        .FirstOrDefaultAsync(c => c.Phone == customer.Phone && c.Id != customer.Id);
                    
                    if (existingCustomer != null)
                        return (false, "Số điện thoại này đã được sử dụng");
                }

                _context.Update(customer);
                await _context.SaveChangesAsync();
                return (true, "Cập nhật khách hàng thành công!");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await CustomerExistsAsync(customer.Id))
                    return (false, "Không tìm thấy khách hàng");
                else
                    throw;
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi: {ex.Message}");
            }
        }

        public async Task<Customer> GetCustomerForDeleteAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return null;

            return await _context.Customers
                .Include(c => c.Orders)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<(bool Success, string Message)> DeleteCustomerAsync(int id)
        {
            try
            {
                // Chuyển đổi id từ int sang string
                string customerId = id.ToString();
                
                // Kiểm tra xem khách hàng có đơn hàng nào không
                var hasOrders = await _context.Orders.AnyAsync(o => o.CustomerId == customerId);
                if (hasOrders)
                    return (false, "Không thể xóa khách hàng có đơn hàng");

                var customer = await _context.Customers.FindAsync(customerId);
                if (customer == null)
                    return (false, "Không tìm thấy khách hàng");

                _context.Customers.Remove(customer);
                await _context.SaveChangesAsync();
                return (true, "Xóa khách hàng thành công!");
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi: {ex.Message}");
            }
        }

        public async Task<bool> CustomerExistsAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return false;
                
            return await _context.Customers.AnyAsync(c => c.Id == id);
        }
        
        // Overload cho CustomerExistsAsync để chấp nhận int id
        public async Task<bool> CustomerExistsAsync(int id)
        {
            // Chuyển đổi id từ int sang string
            string customerId = id.ToString();
            return await CustomerExistsAsync(customerId);
        }

        public async Task<CustomerDetailsViewModel> GetCustomerDetailsAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return null;

            var customer = await _context.Customers
                .Include(c => c.Orders)
                    .ThenInclude(o => o.OrderItems)
                .Include(c => c.Orders)
                    .ThenInclude(o => o.Table)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (customer == null)
                return null;

            var invoices = await _context.Invoices
                .Include(i => i.Order)
                .Where(i => i.Order.CustomerId == id)
                .ToListAsync();

            decimal totalSpent = invoices.Sum(i => i.TotalAmount);

            // Convert Order to OrderViewModel
            var orderViewModels = customer.Orders?.Select(o => new OrderViewModel
            {
                Id = o.Id,
                TableName = o.Table?.Name,
                CustomerName = customer.Name,
                OrderDate = o.OrderDate,
                Status = o.Status,
                Total = o.OrderItems?.Sum(i => i.Subtotal) ?? 0,
                TotalAmount = o.OrderItems?.Sum(i => i.Subtotal) ?? 0,
                ItemCount = o.OrderItems?.Count ?? 0
            }).ToList() ?? new List<OrderViewModel>();

            var viewModel = new CustomerDetailsViewModel
            {
                Id = customer.Id,
                FullName = customer.Name,
                PhoneNumber = customer.Phone,
                Email = customer.Email,
                Address = customer.Address,
                Notes = customer.Notes,
                OrderCount = customer.Orders?.Count ?? 0,
                TotalSpent = totalSpent,
                Orders = orderViewModels,
                Invoices = invoices
            };

            return viewModel;
        }

        public async Task<List<Order>> GetCustomerOrdersAsync(int customerId)
        {
            // Chuyển đổi customerId từ int sang string
            string customerIdStr = customerId.ToString();
            
            return await _context.Orders
                .Include(o => o.Table)
                .Where(o => o.CustomerId == customerIdStr)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public async Task<SelectList> GetCustomersSelectListAsync(int? selectedCustomerId = null)
        {
            string selectedCustomerIdStr = selectedCustomerId?.ToString();
            
            return new SelectList(
                await _context.Customers.OrderBy(c => c.Name).ToListAsync(),
                "Id",
                "Name",
                selectedCustomerIdStr
            );
        }

        public async Task<Customer> FindCustomerByPhoneAsync(string phone)
        {
            if (string.IsNullOrEmpty(phone))
                return null;

            return await _context.Customers
                .FirstOrDefaultAsync(c => c.Phone == phone);
        }
    }
} 