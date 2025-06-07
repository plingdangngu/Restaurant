using RestaurantManagement.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RestaurantManagement.Repositories.Interfaces
{
    public interface ICustomerRepository
    {
        Task<List<Customer>> GetAllAsync();
        Task<Customer> GetByIdAsync(string id);
        Task<Customer> GetByIdWithDetailsAsync(string id);
        Task<bool> ExistsAsync(string id);
        Task<bool> ExistsByEmailAsync(string email);
        Task AddAsync(Customer customer);
        void Update(Customer customer);
        void Remove(Customer customer);
        Task<List<Order>> GetOrdersByCustomerIdAsync(string customerId);
        Task<List<Reservation>> GetReservationsByCustomerIdAsync(string customerId);
        Task<Customer> GetByEmailAsync(string email);
        Task<Customer> GetByPhoneNumberAsync(string phoneNumber);
        Task SaveChangesAsync();
    }
} 