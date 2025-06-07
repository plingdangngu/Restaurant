using RestaurantManagement.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RestaurantManagement.Repositories.Interfaces
{
    public interface ITableRepository
    {
        Task<List<Table>> GetAllAsync();
        Task<Table> GetByIdAsync(int id);
        Task<Table> GetByIdWithFloorAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task AddAsync(Table table);
        void Update(Table table);
        void Remove(Table table);
        Task<List<Table>> GetAvailableTablesAsync(int? capacity = null);
        Task<Dictionary<int, List<Table>>> GetTablesGroupedByFloorAsync();
        Task<bool> HasActiveOrdersAsync(int tableId);
        Task<List<Floor>> GetFloorsAsync();
        Task MoveOrdersAsync(int sourceTableId, int targetTableId);
        Task SaveChangesAsync();
    }
} 