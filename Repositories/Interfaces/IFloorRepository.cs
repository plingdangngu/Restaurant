using RestaurantManagement.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RestaurantManagement.Repositories.Interfaces
{
    public interface IFloorRepository
    {
        Task<List<Floor>> GetAllAsync();
        Task<Floor> GetByIdAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<bool> NameExistsAsync(string name, int? excludeId = null);
        Task AddAsync(Floor floor);
        void Update(Floor floor);
        void Remove(Floor floor);
        Task SaveChangesAsync();
        Task<bool> HasTablesAsync(int id);
    }
} 