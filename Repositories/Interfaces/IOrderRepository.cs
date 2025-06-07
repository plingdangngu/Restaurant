using RestaurantManagement.Models;
using RestaurantManagement.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace RestaurantManagement.Repositories.Interfaces
{
    public interface IOrderRepository
    {
        Task<List<Order>> GetAllAsync();
        Task<List<Order>> GetAllAsync(Expression<Func<Order, bool>> predicate);
        Task<Order> GetByIdAsync(int id);
        Task<Order> GetByIdWithDetailsAsync(int id);
        Task<Order> GetForEditAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<bool> ExistsAsync(Expression<Func<Order, bool>> predicate);
        Task AddAsync(Order order);
        Task<T> AddAsync<T>(T entity) where T : class;
        void Add<T>(T entity) where T : class;
        void Update(Order order);
        void Update<T>(T entity) where T : class;
        void Remove(Order order);
        void Remove<T>(T entity) where T : class;
        void RemoveRange<T>(IEnumerable<T> entities) where T : class;
        Task<List<Order>> GetOrdersByTableIdAsync(int tableId);
        Task<List<Order>> GetOrdersByStatusAsync(string status);
        Task<List<Order>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<List<Order>> GetOrdersByCustomerIdAsync(string customerId);
        Task<List<OrderItem>> GetOrderItemsByOrderIdAsync(int orderId);
        Task<MenuItem> GetMenuItemByIdAsync(int id);
        Task AddOrderItemAsync(OrderItem item);
        void RemoveOrderItem(OrderItem item);
        Task<OrderItem> GetOrderItemByIdAsync(int id);
        Task<List<OrderViewModel>> GetOrdersForListAsync();
        Task<bool> HasActiveOrdersForTableAsync(int tableId);
        Task<T> GetFirstOrDefaultAsync<T>(Expression<Func<T, bool>> predicate) where T : class;
        Task<List<T>> GetAllAsync<T>(Expression<Func<T, bool>> predicate) where T : class;
        Task<int> CountAsync<T>(Expression<Func<T, bool>> predicate) where T : class;
        Task SaveChangesAsync();
    }
} 