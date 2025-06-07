using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Data;
using RestaurantManagement.Models;
using RestaurantManagement.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantManagement.Services.Implementations
{
    public class FloorService : IFloorService
    {
        private readonly ApplicationDbContext _context;

        public FloorService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Floor>> GetAllFloorsAsync()
        {
            return await _context.Floors
                .OrderBy(f => f.DisplayOrder)
                .ToListAsync();
        }

        public async Task<Floor> GetFloorByIdAsync(int id)
        {
            return await _context.Floors
                .FirstOrDefaultAsync(f => f.Id == id);
        }
        
        public async Task<Floor> GetFloorWithTablesAsync(int id)
        {
            return await _context.Floors
                .Include(f => f.Tables)
                .FirstOrDefaultAsync(f => f.Id == id);
        }

        public async Task<(bool Success, string Message, Floor Floor)> CreateFloorAsync(Floor floor)
        {
            if (floor == null)
                return (false, "Tầng không hợp lệ", null);

            try
            {
                _context.Add(floor);
                await _context.SaveChangesAsync();
                return (true, "Tạo tầng thành công!", floor);
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi: {ex.Message}", null);
            }
        }

        public async Task<(bool Success, string Message)> UpdateFloorAsync(Floor floor)
        {
            if (floor == null)
                return (false, "Tầng không hợp lệ");

            try
            {
                _context.Update(floor);
                await _context.SaveChangesAsync();
                return (true, "Cập nhật tầng thành công!");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await FloorExistsAsync(floor.Id))
                    return (false, "Không tìm thấy tầng");
                else
                    throw;
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi: {ex.Message}");
            }
        }

        public async Task<Floor> GetFloorForDeleteAsync(int id)
        {
            // Lấy thông tin tầng kèm theo danh sách bàn
            return await _context.Floors
                .Include(f => f.Tables)
                .FirstOrDefaultAsync(f => f.Id == id);
        }

        public async Task<(bool Success, string Message)> DeleteFloorAsync(int id)
        {
            try
            {
                // Kiểm tra xem tầng có chứa bàn nào không
                var tableCount = await GetTableCountByFloorIdAsync(id);
                if (tableCount > 0)
                    return (false, $"Không thể xóa tầng này vì có {tableCount} bàn thuộc tầng");

                var floor = await _context.Floors.FindAsync(id);
                if (floor == null)
                    return (false, "Không tìm thấy tầng");

                _context.Floors.Remove(floor);
                await _context.SaveChangesAsync();
                return (true, "Xóa tầng thành công!");
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi: {ex.Message}");
            }
        }

        public async Task<bool> FloorExistsAsync(int id)
        {
            return await _context.Floors.AnyAsync(f => f.Id == id);
        }

        public async Task<int> GetTableCountByFloorIdAsync(int floorId)
        {
            return await _context.Tables
                .CountAsync(t => t.FloorId == floorId);
        }
        
        public async Task<SelectList> GetFloorsSelectListAsync(int? selectedFloorId = null)
        {
            return new SelectList(
                await _context.Floors.OrderBy(f => f.DisplayOrder).ToListAsync(),
                "Id",
                "Name",
                selectedFloorId
            );
        }
    }
} 