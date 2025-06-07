using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Data;
using RestaurantManagement.Models;
using RestaurantManagement.Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantManagement.Repositories.Implementations
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly ApplicationDbContext _context;

        public CustomerRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Customer>> GetAllAsync()
        {
            return await _context.Customers
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<Customer> GetByIdAsync(string id)
        {
            return await _context.Customers
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Customer> GetByIdWithDetailsAsync(string id)
        {
            return await _context.Customers
                .Include(c => c.Orders)
                .Include(c => c.Reservations)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<bool> ExistsAsync(string id)
        {
            return await _context.Customers
                .AnyAsync(c => c.Id == id);
        }

        public async Task<bool> ExistsByEmailAsync(string email)
        {
            return await _context.Customers
                .AnyAsync(c => c.Email == email);
        }

        public async Task AddAsync(Customer customer)
        {
            await _context.Customers.AddAsync(customer);
        }

        public void Update(Customer customer)
        {
            _context.Customers.Update(customer);
        }

        public void Remove(Customer customer)
        {
            _context.Customers.Remove(customer);
        }

        public async Task<List<Order>> GetOrdersByCustomerIdAsync(string customerId)
        {
            return await _context.Orders
                .Include(o => o.Table)
                .Where(o => o.CustomerId == customerId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public async Task<List<Reservation>> GetReservationsByCustomerIdAsync(string customerId)
        {
            return await _context.Reservations
                .Include(r => r.Table)
                .Where(r => r.CustomerId == customerId)
                .OrderByDescending(r => r.ReservationDate)
                .ThenBy(r => r.ReservationTime)
                .ToListAsync();
        }

        public async Task<Customer> GetByEmailAsync(string email)
        {
            return await _context.Customers
                .FirstOrDefaultAsync(c => c.Email == email);
        }

        public async Task<Customer> GetByPhoneNumberAsync(string phoneNumber)
        {
            return await _context.Customers
                .FirstOrDefaultAsync(c => c.PhoneNumber == phoneNumber);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
} 