using Microsoft.AspNetCore.Mvc.Rendering;
using RestaurantManagement.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RestaurantManagement.Services.Interfaces
{
    public interface IFloorService
    {
        Task<List<Floor>> GetAllFloorsAsync();
        Task<Floor> GetFloorByIdAsync(int id);
        Task<Floor> GetFloorWithTablesAsync(int id);
        Task<(bool Success, string Message, Floor Floor)> CreateFloorAsync(Floor floor);
        Task<(bool Success, string Message)> UpdateFloorAsync(Floor floor);
        Task<Floor> GetFloorForDeleteAsync(int id);
        Task<(bool Success, string Message)> DeleteFloorAsync(int id);
        Task<bool> FloorExistsAsync(int id);
        Task<int> GetTableCountByFloorIdAsync(int floorId);
        Task<SelectList> GetFloorsSelectListAsync(int? selectedFloorId = null);
    }
} 