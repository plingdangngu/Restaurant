using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Data;
using RestaurantManagement.Models;
using RestaurantManagement.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantManagement.Repositories.Implementations
{
    public class ReservationRepository : IReservationRepository
    {
        private readonly ApplicationDbContext _context;

        public ReservationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Reservation>> GetAllAsync()
        {
            return await _context.Reservations
                .Include(r => r.Customer)
                .Include(r => r.Table)
                .OrderByDescending(r => r.ReservationDate)
                .ThenBy(r => r.ReservationTime)
                .ToListAsync();
        }

        public async Task<Reservation> GetByIdAsync(int id)
        {
            return await _context.Reservations.FindAsync(id);
        }

        public async Task<Reservation> GetByIdWithDetailsAsync(int id)
        {
            return await _context.Reservations
                .Include(r => r.Customer)
                .Include(r => r.Table)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Reservations.AnyAsync(r => r.Id == id);
        }

        public async Task AddAsync(Reservation reservation)
        {
            await _context.Reservations.AddAsync(reservation);
        }

        public void Update(Reservation reservation)
        {
            _context.Reservations.Update(reservation);
        }

        public void Remove(Reservation reservation)
        {
            _context.Reservations.Remove(reservation);
        }

        public async Task<List<Reservation>> GetByCustomerIdAsync(string customerId)
        {
            return await _context.Reservations
                .Include(r => r.Table)
                .Where(r => r.CustomerId == customerId)
                .OrderByDescending(r => r.ReservationDate)
                .ThenBy(r => r.ReservationTime)
                .ToListAsync();
        }

        public async Task<List<Reservation>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Reservations
                .Include(r => r.Customer)
                .Include(r => r.Table)
                .Where(r => r.ReservationDate >= startDate && r.ReservationDate <= endDate)
                .OrderBy(r => r.ReservationDate)
                .ThenBy(r => r.ReservationTime)
                .ToListAsync();
        }

        public async Task<List<Reservation>> GetUpcomingReservationsAsync()
        {
            var today = DateTime.Today;
            return await _context.Reservations
                .Include(r => r.Customer)
                .Include(r => r.Table)
                .Where(r => r.ReservationDate >= today && r.Status == "Confirmed")
                .OrderBy(r => r.ReservationDate)
                .ThenBy(r => r.ReservationTime)
                .ToListAsync();
        }

        public async Task<List<Reservation>> GetReservationsByStatusAsync(string status)
        {
            return await _context.Reservations
                .Include(r => r.Customer)
                .Include(r => r.Table)
                .Where(r => r.Status == status)
                .OrderBy(r => r.ReservationDate)
                .ThenBy(r => r.ReservationTime)
                .ToListAsync();
        }

        public async Task<bool> IsTableReservedAsync(int tableId, DateTime date, TimeSpan startTime, TimeSpan endTime, int? excludeReservationId = null)
        {
            // Tính toán thời gian kết thúc dự kiến (thêm 2 giờ từ thời gian bắt đầu)
            var query = _context.Reservations
                .Where(r => r.TableId == tableId && 
                            r.ReservationDate.Date == date.Date && 
                            r.Status != "Canceled");
                            
            // Kiểm tra xem có đặt chỗ nào trùng thời gian không
            // Giả định rằng mỗi đặt chỗ kéo dài 2 giờ
            query = query.Where(r => (r.ReservationTime <= startTime && r.ReservationTime.Add(TimeSpan.FromHours(2)) > startTime) || 
                                    (r.ReservationTime < endTime && r.ReservationTime.Add(TimeSpan.FromHours(2)) >= endTime) || 
                                    (r.ReservationTime >= startTime && r.ReservationTime.Add(TimeSpan.FromHours(2)) <= endTime));

            if (excludeReservationId.HasValue)
                query = query.Where(r => r.Id != excludeReservationId.Value);

            return await query.AnyAsync();
        }

        public async Task<List<Table>> GetAvailableTablesAsync(DateTime date, TimeSpan startTime, TimeSpan endTime, int partySize)
        {
            // First get tables that can accommodate the party size
            var tables = await _context.Tables
                .Include(t => t.Floor)
                .Where(t => t.Capacity >= partySize && t.Status == "Available")
                .ToListAsync();

            // Filter out tables that are already reserved
            var result = new List<Table>();
            foreach (var table in tables)
            {
                if (!await IsTableReservedAsync(table.Id, date, startTime, endTime))
                {
                    result.Add(table);
                }
            }

            return result.OrderBy(t => t.Capacity).ToList();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
} 