using RestaurantManagement.Models;
using RestaurantManagement.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RestaurantManagement.Repositories.Interfaces
{
    public interface IReservationRepository
    {
        Task<List<Reservation>> GetAllAsync();
        Task<Reservation> GetByIdAsync(int id);
        Task<Reservation> GetByIdWithDetailsAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task AddAsync(Reservation reservation);
        void Update(Reservation reservation);
        void Remove(Reservation reservation);
        Task<List<Reservation>> GetByCustomerIdAsync(string customerId);
        Task<List<Reservation>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<List<Reservation>> GetUpcomingReservationsAsync();
        Task<List<Reservation>> GetReservationsByStatusAsync(string status);
        Task<bool> IsTableReservedAsync(int tableId, DateTime date, TimeSpan startTime, TimeSpan endTime, int? excludeReservationId = null);
        Task<List<Table>> GetAvailableTablesAsync(DateTime date, TimeSpan startTime, TimeSpan endTime, int partySize);
        Task SaveChangesAsync();
    }
} 