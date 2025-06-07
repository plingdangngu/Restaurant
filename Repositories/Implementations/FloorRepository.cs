using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Data;
using RestaurantManagement.Models;
using RestaurantManagement.Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantManagement.Repositories.Implementations
{
    public class FloorRepository : IFloorRepository
    {
        private readonly ApplicationDbContext _context;

        public FloorRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Floor>> GetAllAsync()
        {
            return await _context.Floors
                .OrderBy(f => f.DisplayOrder)
                .ToListAsync();
        }

        public async Task<Floor> GetByIdAsync(int id)
        {
            return await _context.Floors
                .FindAsync(id);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Floors
                .AnyAsync(f => f.Id == id);
        }

        public async Task<bool> NameExistsAsync(string name, int? excludeId = null)
        {
            if (excludeId.HasValue)
                return await _context.Floors
                    .AnyAsync(f => f.Name == name && f.Id != excludeId.Value);
            else
                return await _context.Floors
                    .AnyAsync(f => f.Name == name);
        }

        public async Task AddAsync(Floor floor)
        {
            await _context.Floors.AddAsync(floor);
        }

        public void Update(Floor floor)
        {
            _context.Floors.Update(floor);
        }

        public void Remove(Floor floor)
        {
            _context.Floors.Remove(floor);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<bool> HasTablesAsync(int id)
        {
            return await _context.Tables
                .AnyAsync(t => t.FloorId == id);
        }
    }
} 