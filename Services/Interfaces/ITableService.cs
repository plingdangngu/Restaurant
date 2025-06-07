using Microsoft.AspNetCore.Mvc.Rendering;
using RestaurantManagement.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RestaurantManagement.Services.Interfaces
{
    public interface ITableService
    {
        Task<List<Table>> GetAllTablesAsync();
        Task<Table> GetTableByIdAsync(int id);
        Task<(bool Success, string Message)> CreateTableAsync(Table table);
        Task<(bool Success, string Message)> UpdateTableAsync(Table table);
        Task<Table> GetTableForDeleteAsync(int id);
        Task<(bool Success, string Message)> DeleteTableAsync(int id);
        Task<bool> TableExistsAsync(int id);
        Task<List<Table>> GetAvailableTablesAsync(int? capacity = null);
        Task<SelectList> GetFloorsSelectListAsync(int? selectedFloorId = null);
        Task<Dictionary<int, List<Table>>> GetTablesGroupedByFloorAsync();
        Task<List<Floor>> GetFloorsAsync();
        Task MoveOrdersAsync(int sourceTableId, int targetTableId);
        Task SaveChangesAsync();
    }
} 