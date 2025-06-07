using Microsoft.AspNetCore.Mvc.Rendering;
using RestaurantManagement.Models;
using RestaurantManagement.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RestaurantManagement.Services.Interfaces
{
    public interface ICustomerService
    {
        Task<List<Customer>> GetAllCustomersAsync();
        Task<Customer> GetCustomerByIdAsync(int id);
        Task<CustomerViewModel> PrepareCreateViewModelAsync();
        Task<(bool Success, string Message, Customer Customer)> CreateCustomerAsync(Customer customer);
        Task<CustomerViewModel> PrepareEditViewModelAsync(string id);
        Task<(bool Success, string Message)> UpdateCustomerAsync(Customer customer);
        Task<Customer> GetCustomerForDeleteAsync(string id);
        Task<(bool Success, string Message)> DeleteCustomerAsync(int id);
        Task<bool> CustomerExistsAsync(int id);
        Task<CustomerDetailsViewModel> GetCustomerDetailsAsync(string id);
        Task<List<Order>> GetCustomerOrdersAsync(int customerId);
        Task<SelectList> GetCustomersSelectListAsync(int? selectedCustomerId = null);
        Task<Customer> FindCustomerByPhoneAsync(string phone);
    }
} 