using Microsoft.AspNetCore.Mvc.Rendering;
using RestaurantManagement.Models;
using RestaurantManagement.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RestaurantManagement.Services.Interfaces
{
    public interface IOrderService
    {
        Task<List<Order>> GetAllOrdersAsync();
        Task<Order> GetOrderByIdAsync(int id);
        Task<Order> GetOrderWithDetailsAsync(int id);
        Task<Order> GetOrderForDetailsAsync(int id);
        Task<OrderCreateViewModel> PrepareCreateViewModelAsync();
        Task<(bool Success, string Message, Order Order)> CreateOrderAsync(OrderCreateViewModel model, string userId);
        Task<OrderEditViewModel> PrepareEditViewModelAsync(int id);
        Task<(bool Success, string Message)> UpdateOrderAsync(OrderEditViewModel model);
        Task<Order> GetOrderForDeleteAsync(int id);
        Task<(bool Success, string Message)> DeleteOrderAsync(int id);
        Task<bool> OrderExistsAsync(int id);
        Task<List<Order>> GetPendingOrdersAsync();
        Task<List<Order>> GetOrdersByTableAsync(int tableId);
        Task<AddItemViewModel> PrepareAddItemViewModelAsync(int orderId);
        Task<(bool Success, string Message)> AddItemToOrderAsync(AddItemViewModel model);
        Task<(bool Success, string Message)> RemoveItemFromOrderAsync(int id);
        Task<(bool Success, string Message)> AddOrderItemAsync(OrderItem item);
        Task<(bool Success, string Message)> UpdateOrderItemAsync(OrderItem item);
        Task<(bool Success, string Message)> DeleteOrderItemAsync(int itemId);
        Task<List<OrderItem>> GetOrderItemsAsync(int orderId);
        Task<Order> GetOrderForPaymentAsync(int id);
        Task<(bool Success, string Message)> ProcessPaymentAsync(int orderId, PaymentViewModel payment);
        Task<(bool Success, string Message, Invoice Invoice)> CreatePaymentAsync(int orderId);
        Task<(bool Success, string Message)> ChangeOrderStatusAsync(int orderId, string status);
        Task<decimal> CalculateOrderTotalAsync(int orderId);
        Task<List<object>> GetActiveOrdersAsync();
        Task<OrderViewModel> PrepareOrderViewModelAsync(int? orderId, int? tableId);
        Task<List<Order>> GetUserOrdersAsync(string userId);
        Task<Table> GetTableByIdAsync(int tableId);
        
        // Helper methods for SelectLists
        Task<SelectList> GetTablesSelectListAsync(int? selectedTableId = null);
        Task<SelectList> GetCustomersSelectListAsync();
        Task<SelectList> GetMenuItemsSelectListAsync();
        Task<OrderItem> GetOrderItemAsync(int id);
    }
} 