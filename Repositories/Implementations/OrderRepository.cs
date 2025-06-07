using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Data;
using RestaurantManagement.Models;
using RestaurantManagement.Repositories.Interfaces;
using RestaurantManagement.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace RestaurantManagement.Repositories.Implementations
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext _context;

        public OrderRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Order>> GetAllAsync()
        {
            return await _context.Orders
                .Include(o => o.Table)
                .Include(o => o.Customer)
                .Include(o => o.Employee)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public async Task<List<Order>> GetAllAsync(Expression<Func<Order, bool>> predicate)
        {
            return await _context.Orders
                .Include(o => o.Table)
                .Include(o => o.Customer)
                .Include(o => o.Employee)
                .Where(predicate)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public async Task<Order> GetByIdAsync(int id)
        {
            return await _context.Orders.FindAsync(id);
        }

        public async Task<Order> GetByIdWithDetailsAsync(int id)
        {
            return await _context.Orders
                .Include(o => o.Table)
                .Include(o => o.Customer)
                .Include(o => o.Employee)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.MenuItem)
                        .ThenInclude(mi => mi.Category)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<Order> GetForEditAsync(int id)
        {
            return await _context.Orders
                .Include(o => o.Table)
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Orders.AnyAsync(o => o.Id == id);
        }

        public async Task<bool> ExistsAsync(Expression<Func<Order, bool>> predicate)
        {
            return await _context.Orders.AnyAsync(predicate);
        }

        public async Task AddAsync(Order order)
        {
            await _context.Orders.AddAsync(order);
        }

        public async Task<T> AddAsync<T>(T entity) where T : class
        {
            await _context.Set<T>().AddAsync(entity);
            return entity;
        }

        public void Add<T>(T entity) where T : class
        {
            _context.Set<T>().Add(entity);
        }

        public void Update(Order order)
        {
            _context.Orders.Update(order);
        }

        public void Update<T>(T entity) where T : class
        {
            _context.Set<T>().Update(entity);
        }

        public void Remove(Order order)
        {
            _context.Orders.Remove(order);
        }

        public void Remove<T>(T entity) where T : class
        {
            _context.Set<T>().Remove(entity);
        }

        public void RemoveRange<T>(IEnumerable<T> entities) where T : class
        {
            _context.Set<T>().RemoveRange(entities);
        }

        public async Task<List<Order>> GetOrdersByTableIdAsync(int tableId)
        {
            return await _context.Orders
                .Where(o => o.TableId == tableId && o.Status != "Completed" && o.Status != "Canceled")
                .ToListAsync();
        }

        public async Task<List<Order>> GetOrdersByStatusAsync(string status)
        {
            return await _context.Orders
                .Include(o => o.Table)
                .Include(o => o.Employee)
                .Where(o => o.Status == status)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public async Task<List<Order>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Orders
                .Include(o => o.Table)
                .Include(o => o.Employee)
                .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public async Task<List<Order>> GetOrdersByCustomerIdAsync(string customerId)
        {
            return await _context.Orders
                .Include(o => o.Table)
                .Where(o => o.CustomerId == customerId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public async Task<List<OrderItem>> GetOrderItemsByOrderIdAsync(int orderId)
        {
            return await _context.OrderItems
                .Include(oi => oi.MenuItem)
                .Where(oi => oi.OrderId == orderId)
                .ToListAsync();
        }

        public async Task<MenuItem> GetMenuItemByIdAsync(int id)
        {
            return await _context.MenuItems
                .Include(mi => mi.Category)
                .FirstOrDefaultAsync(mi => mi.Id == id);
        }

        public async Task AddOrderItemAsync(OrderItem item)
        {
            await _context.OrderItems.AddAsync(item);
        }

        public void RemoveOrderItem(OrderItem item)
        {
            _context.OrderItems.Remove(item);
        }

        public async Task<OrderItem> GetOrderItemByIdAsync(int id)
        {
            return await _context.OrderItems
                .Include(oi => oi.MenuItem)
                .FirstOrDefaultAsync(oi => oi.Id == id);
        }

        public async Task<List<OrderViewModel>> GetOrdersForListAsync()
        {
            var orders = await _context.Orders
                .Include(o => o.Table)
                .Include(o => o.Customer)
                .Include(o => o.Employee)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.MenuItem)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return orders.Select(o => new OrderViewModel
            {
                Id = o.Id,
                TableName = o.Table?.Name,
                CustomerName = o.Customer?.FullName,
                EmployeeName = o.Employee?.FullName,
                OrderDate = o.OrderDate,
                TotalAmount = o.OrderItems.Sum(oi => oi.Quantity * oi.UnitPrice),
                Status = o.Status,
                ItemCount = o.OrderItems.Count
            }).ToList();
        }

        public async Task<bool> HasActiveOrdersForTableAsync(int tableId)
        {
            return await _context.Orders
                .AnyAsync(o => o.TableId == tableId && 
                         (o.Status == "Pending" || o.Status == "Processing"));
        }

        public async Task<T> GetFirstOrDefaultAsync<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            return await _context.Set<T>().FirstOrDefaultAsync(predicate);
        }

        public async Task<List<T>> GetAllAsync<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            return await _context.Set<T>().Where(predicate).ToListAsync();
        }

        public async Task<int> CountAsync<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            return await _context.Set<T>().CountAsync(predicate);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
} 