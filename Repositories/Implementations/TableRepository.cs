using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Data;
using RestaurantManagement.Models;
using RestaurantManagement.Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantManagement.Repositories.Implementations
{
    public class TableRepository : ITableRepository
    {
        private readonly ApplicationDbContext _context;

        public TableRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Table>> GetAllAsync()
        {
            return await _context.Tables
                .Include(t => t.Floor)
                .OrderBy(t => t.Floor.Name)
                .ThenBy(t => t.Name)
                .ToListAsync();
        }

        public async Task<Table> GetByIdAsync(int id)
        {
            return await _context.Tables.FindAsync(id);
        }

        public async Task<Table> GetByIdWithFloorAsync(int id)
        {
            return await _context.Tables
                .Include(t => t.Floor)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Tables.AnyAsync(t => t.Id == id);
        }

        public async Task AddAsync(Table table)
        {
            await _context.Tables.AddAsync(table);
        }

        public void Update(Table table)
        {
            _context.Tables.Update(table);
        }

        public void Remove(Table table)
        {
            _context.Tables.Remove(table);
        }

        public async Task<List<Table>> GetAvailableTablesAsync(int? capacity = null)
        {
            var query = _context.Tables
                .Include(t => t.Floor)
                .Where(t => !t.IsOccupied);

            if (capacity.HasValue)
                query = query.Where(t => t.Capacity >= capacity.Value);

            return await query
                .OrderBy(t => t.Floor.Name)
                .ThenBy(t => t.Name)
                .ToListAsync();
        }

        public async Task<Dictionary<int, List<Table>>> GetTablesGroupedByFloorAsync()
        {
            var tables = await _context.Tables
                .Include(t => t.Floor)
                .OrderBy(t => t.Floor.DisplayOrder)
                .ThenBy(t => t.Name)
                .ToListAsync();

            return tables
                .GroupBy(t => t.FloorId)
                .ToDictionary(g => g.Key, g => g.ToList());
        }

        public async Task<bool> HasActiveOrdersAsync(int tableId)
        {
            return await _context.Orders
                .AnyAsync(o => o.TableId == tableId && 
                         (o.Status == "Pending" || o.Status == "Processing"));
        }

        public async Task<List<Floor>> GetFloorsAsync()
        {
            return await _context.Floors
                .OrderBy(f => f.DisplayOrder)
                .ToListAsync();
        }

        public async Task MoveOrdersAsync(int sourceTableId, int targetTableId)
        {
            var ordersToMove = await _context.Orders
                .Where(o => o.TableId == sourceTableId && 
                       o.Status != "Completed" && o.Status != "Canceled")
                .ToListAsync();
                
            foreach (var order in ordersToMove)
            {
                order.TableId = targetTableId;
            }
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
} 