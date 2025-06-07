using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Models;
using RestaurantManagement.Repositories.Interfaces;
using RestaurantManagement.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RestaurantManagement.Services.Implementations
{
    public class TableService : ITableService
    {
        private readonly ITableRepository _tableRepository;

        public TableService(ITableRepository tableRepository)
        {
            _tableRepository = tableRepository;
        }

        public async Task<List<Table>> GetAllTablesAsync()
        {
            return await _tableRepository.GetAllAsync();
        }

        public async Task<Table> GetTableByIdAsync(int id)
        {
            return await _tableRepository.GetByIdWithFloorAsync(id);
        }

        public async Task<(bool Success, string Message)> CreateTableAsync(Table table)
        {
            if (table == null)
                return (false, "Bàn không hợp lệ");

            try
            {
                // Kiểm tra tầng có tồn tại không
                var floors = await _tableRepository.GetFloorsAsync();
                if (!floors.Exists(f => f.Id == table.FloorId))
                    return (false, "Tầng không tồn tại");

                await _tableRepository.AddAsync(table);
                await _tableRepository.SaveChangesAsync();
                return (true, "Tạo bàn thành công!");
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi: {ex.Message}");
            }
        }

        public async Task<(bool Success, string Message)> UpdateTableAsync(Table table)
        {
            if (table == null)
                return (false, "Bàn không hợp lệ");

            try
            {
                _tableRepository.Update(table);
                await _tableRepository.SaveChangesAsync();
                return (true, "Cập nhật bàn thành công!");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await TableExistsAsync(table.Id))
                    return (false, "Không tìm thấy bàn");
                else
                    throw;
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi: {ex.Message}");
            }
        }

        public async Task<Table> GetTableForDeleteAsync(int id)
        {
            return await _tableRepository.GetByIdWithFloorAsync(id);
        }

        public async Task<(bool Success, string Message)> DeleteTableAsync(int id)
        {
            try
            {
                // Kiểm tra xem bàn có đang được sử dụng không
                var hasOrders = await _tableRepository.HasActiveOrdersAsync(id);

                if (hasOrders)
                    return (false, "Không thể xóa bàn đang có đơn hàng đang xử lý");

                var table = await _tableRepository.GetByIdAsync(id);
                if (table == null)
                    return (false, "Không tìm thấy bàn");

                _tableRepository.Remove(table);
                await _tableRepository.SaveChangesAsync();
                return (true, "Xóa bàn thành công!");
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi: {ex.Message}");
            }
        }

        public async Task<bool> TableExistsAsync(int id)
        {
            return await _tableRepository.ExistsAsync(id);
        }

        public async Task<List<Table>> GetAvailableTablesAsync(int? capacity = null)
        {
            return await _tableRepository.GetAvailableTablesAsync(capacity);
        }

        public async Task<SelectList> GetFloorsSelectListAsync(int? selectedFloorId = null)
        {
            var floors = await _tableRepository.GetFloorsAsync();
            return new SelectList(
                floors,
                "Id",
                "Name",
                selectedFloorId
            );
        }

        public async Task<Dictionary<int, List<Table>>> GetTablesGroupedByFloorAsync()
        {
            return await _tableRepository.GetTablesGroupedByFloorAsync();
        }

        public async Task<List<Floor>> GetFloorsAsync()
        {
            return await _tableRepository.GetFloorsAsync();
        }

        public async Task MoveOrdersAsync(int sourceTableId, int targetTableId)
        {
            await _tableRepository.MoveOrdersAsync(sourceTableId, targetTableId);
            await _tableRepository.SaveChangesAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _tableRepository.SaveChangesAsync();
        }
    }
} 