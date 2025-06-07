using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Data;
using RestaurantManagement.Models;
using RestaurantManagement.Services.Interfaces;
using RestaurantManagement.ViewModels;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantManagement.Services.Implementations
{
    public class ReservationService : IReservationService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ReservationService(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<List<Order>> GetAllReservationsAsync()
        {
            return await _context.Orders
                .Include(o => o.Table)
                .Include(o => o.Customer)
                .OrderByDescending(o => o.OrderTime)
                .ToListAsync();
        }

        public async Task<Order> GetReservationByIdAsync(int id, string userId)
        {
            var isUserAdmin = await IsUserInRoleAsync(userId, "Admin");
            var isUserEmployee = await IsUserInRoleAsync(userId, "Employee");
            
            // Nếu là Admin hoặc Employee, cho phép truy cập mọi đơn đặt
            if (isUserAdmin || isUserEmployee)
            {
                return await GetReservationByIdAsync(id);
            }
            
            // Nếu là user thường, chỉ cho phép xem đơn đặt của họ
            return await _context.Orders
                .Include(o => o.Table)
                    .ThenInclude(t => t.Floor)
                .Include(o => o.Customer)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.MenuItem)
                .FirstOrDefaultAsync(o => o.Id == id && (o.UserId == userId || o.EmployeeId == userId));
        }

        public async Task<Order> GetReservationByIdAsync(int id)
        {
            return await _context.Orders
                .Include(o => o.Table)
                    .ThenInclude(t => t.Floor)
                .Include(o => o.Customer)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.MenuItem)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<ReservationViewModel> PrepareCreateModelAsync(int? peopleCount)
        {
            int people = peopleCount ?? 2;
            var availableTables = await GetAvailableTablesAsync(people);
            var tableList = new List<Table>();
            
            // Convert dynamic objects to Table objects for the view model
            foreach (dynamic tableObj in availableTables)
            {
                var table = new Table
                {
                    Id = tableObj.id,
                    Name = tableObj.name,
                    Capacity = tableObj.capacity,
                    Floor = new Floor { Name = tableObj.floor.name }
                };
                tableList.Add(table);
            }
            
            return new ReservationViewModel
            {
                ReservationTime = DateTime.Now,
                PeopleCount = people,
                AvailableTables = tableList
            };
        }

        public async Task<(bool Success, string ErrorMessage, Order Reservation)> CreateReservationAsync(ReservationViewModel model, string userId)
        {
            if (model == null)
                return (false, "Model không hợp lệ", null);

            try
            {
                // Get user
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                    return (false, "Không tìm thấy thông tin người dùng", null);

                // Get or create customer record
                var customer = await _context.Customers
                    .FirstOrDefaultAsync(c => c.UserId == userId);

                if (customer == null)
                {
                    customer = new Customer
                    {
                        Name = user.FullName ?? "Khách hàng mới",
                        Email = user.Email,
                        Phone = !string.IsNullOrEmpty(user.PhoneNumber) ? user.PhoneNumber : "0000000000",
                        Address = user.Address,
                        CreatedDate = DateTime.Now,
                        UserId = userId
                    };

                    _context.Add(customer);
                    await _context.SaveChangesAsync();
                }

                // Get table
                var table = await _context.Tables.FindAsync(model.TableId);
                if (table == null)
                    return (false, "Bàn không tồn tại", null);

                if (table.IsOccupied)
                    return (false, "Bàn này đã được đặt", null);

                if (table.Capacity < model.PeopleCount)
                    return (false, "Bàn này không đủ chỗ ngồi", null);

                // Create order
                var order = new Order
                {
                    CustomerId = customer.Id,
                    EmployeeId = userId, // Employee ID
                    UserId = userId,     // User ID
                    TableId = model.TableId,
                    OrderTime = DateTime.Now,
                    OrderDate = DateTime.Now.Date,
                    Status = "Pending",
                    PeopleCount = model.PeopleCount,
                    Notes = model.Notes
                };

                // Update table status
                table.IsOccupied = true;
                table.Status = "Reserved";

                _context.Add(order);
                _context.Update(table);

                await _context.SaveChangesAsync();

                return (true, "Đặt bàn thành công!", order);
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi: {ex.Message}", null);
            }
        }

        public async Task<ReservationEditViewModel> PrepareEditModelAsync(int id)
        {
            var order = await _context.Orders
                .Include(o => o.Table)
                .Include(o => o.Customer)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                return null;

            var viewModel = new ReservationEditViewModel
            {
                Id = order.Id,
                TableId = order.TableId,
                CustomerId = order.CustomerId,
                PeopleCount = order.PeopleCount ?? 1,
                Notes = order.Notes,
                Status = order.Status ?? "Pending"
            };

            return viewModel;
        }

        public async Task<(bool Success, string ErrorMessage)> UpdateReservationAsync(ReservationEditViewModel model)
        {
            if (model == null)
                return (false, "Model không hợp lệ");

            try
            {
                var order = await _context.Orders
                    .Include(o => o.Table)
                    .FirstOrDefaultAsync(o => o.Id == model.Id);

                if (order == null)
                    return (false, "Không tìm thấy đơn đặt bàn");

                // If table has changed
                if (order.TableId != model.TableId)
                {
                    // Free old table
                    if (order.Table != null)
                    {
                        order.Table.IsOccupied = false;
                        order.Table.Status = "Available";
                        _context.Update(order.Table);
                    }

                    // Occupy new table
                    var newTable = await _context.Tables.FindAsync(model.TableId);
                    if (newTable != null)
                    {
                        newTable.IsOccupied = true;
                        newTable.Status = "Reserved";
                        _context.Update(newTable);
                    }
                }

                // Update order fields
                order.TableId = model.TableId;
                order.CustomerId = model.CustomerId;
                order.PeopleCount = model.PeopleCount;
                order.Notes = model.Notes;
                order.Status = model.Status;

                // If status has changed to Cancelled or Completed, free the table
                if (model.Status == "Cancelled" || model.Status == "Completed")
                {
                    var table = await _context.Tables.FindAsync(model.TableId);
                    if (table != null)
                    {
                        table.IsOccupied = false;
                        table.Status = "Available";
                        _context.Update(table);
                    }
                }

                _context.Update(order);
                await _context.SaveChangesAsync();

                return (true, "Cập nhật đặt bàn thành công!");
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi: {ex.Message}");
            }
        }

        public async Task<Order> GetReservationForDeleteAsync(int id)
        {
            return await _context.Orders
                .Include(o => o.Table)
                .Include(o => o.Customer)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task DeleteReservationAsync(int id)
        {
            var order = await _context.Orders
                .Include(o => o.Table)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order != null)
            {
                // Free table
                if (order.Table != null)
                {
                    order.Table.IsOccupied = false;
                    order.Table.Status = "Available";
                    _context.Update(order.Table);
                }

                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();
            }
        }

        public async Task ConfirmReservationAsync(int id)
        {
            var order = await _context.Orders
                .Include(o => o.Table)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                return;

            order.Status = "Confirmed";
            _context.Update(order);
            await _context.SaveChangesAsync();
        }

        public async Task<(bool Success, string ErrorMessage)> CancelReservationAsync(int id, string userId)
        {
            var order = await _context.Orders
                .Include(o => o.Table)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                return (false, "Không tìm thấy đơn đặt");

            // Kiểm tra xem user có quyền hủy đơn này không (chỉ hủy đơn của mình)
            if (order.EmployeeId != userId && !await IsUserInRoleAsync(userId, "Admin") && !await IsUserInRoleAsync(userId, "Employee"))
                return (false, "Bạn không có quyền hủy đơn đặt bàn này!");

            // Kiểm tra trạng thái có thể hủy không
            if (order.Status != "Reserved" && order.Status != "Pending")
                return (false, "Không thể hủy đơn đặt bàn với trạng thái hiện tại!");

            order.Status = "Cancelled";

            // Giải phóng bàn
            if (order.Table != null)
            {
                order.Table.IsOccupied = false;
                order.Table.Status = "Available";
                _context.Update(order.Table);
            }

            _context.Update(order);
            await _context.SaveChangesAsync();

            return (true, "Đã hủy đặt bàn thành công!");
        }

        public async Task<List<Order>> GetUserReservationsAsync(string userId)
        {
            return await _context.Orders
                .Include(o => o.Table)
                .Include(o => o.Customer)
                .Include(o => o.OrderItems)
                .Where(o => o.UserId == userId || o.EmployeeId == userId)
                .OrderByDescending(o => o.OrderTime)
                .ToListAsync();
        }

        public async Task<List<dynamic>> GetAvailableTablesAsync(int peopleCount)
        {
            var availableTables = await _context.Tables
                .Include(t => t.Floor)
                .Where(t => !t.IsOccupied && t.Capacity >= peopleCount)
                .OrderBy(t => t.Capacity)
                .ThenBy(t => t.Floor.Name)
                .ThenBy(t => t.Name)
                .Select(t => new
                {
                    id = t.Id,
                    name = t.Name,
                    capacity = t.Capacity,
                    floor = new { name = t.Floor.Name ?? "Không xác định" }
                })
                .ToListAsync();

            // Convert to dynamic for controller return
            List<dynamic> result = new List<dynamic>();
            foreach (var table in availableTables)
            {
                dynamic tableObj = new ExpandoObject();
                tableObj.id = table.id;
                tableObj.name = table.name;
                tableObj.capacity = table.capacity;
                tableObj.floor = table.floor;
                result.Add(tableObj);
            }

            return result;
        }

        public bool ReservationExists(int id)
        {
            return _context.Orders.Any(e => e.Id == id);
        }

        public async Task<SelectList> GetTablesSelectListAsync(int? selectedTableId = null)
        {
            var tables = await _context.Tables.Include(t => t.Floor)
                .Select(t => new
                {
                    t.Id,
                    DisplayName = $"{t.Name} (Tầng {t.Floor.Name}, Sức chứa: {t.Capacity})"
                })
                .ToListAsync();

            return new SelectList(tables, "Id", "DisplayName", selectedTableId);
        }

        public async Task<SelectList> GetCustomersSelectListAsync(string selectedCustomerId = null)
        {
            return new SelectList(await _context.Customers.ToListAsync(), "Id", "Name", selectedCustomerId);
        }

        // Helper methods
        private async Task<bool> IsUserInRoleAsync(string userId, string role)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;
            return await _userManager.IsInRoleAsync(user, role);
        }
    }
} 