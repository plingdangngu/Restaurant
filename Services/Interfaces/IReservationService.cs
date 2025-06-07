using Microsoft.AspNetCore.Mvc.Rendering;
using RestaurantManagement.Models;
using RestaurantManagement.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RestaurantManagement.Services.Interfaces
{
    public interface IReservationService
    {
        Task<List<Order>> GetAllReservationsAsync();
        Task<Order> GetReservationByIdAsync(int id, string userId);
        Task<Order> GetReservationByIdAsync(int id);
        Task<ReservationViewModel> PrepareCreateModelAsync(int? peopleCount);
        Task<(bool Success, string ErrorMessage, Order Reservation)> CreateReservationAsync(ReservationViewModel model, string userId);
        Task<ReservationEditViewModel> PrepareEditModelAsync(int id);
        Task<(bool Success, string ErrorMessage)> UpdateReservationAsync(ReservationEditViewModel model);
        Task<Order> GetReservationForDeleteAsync(int id);
        Task DeleteReservationAsync(int id);
        Task ConfirmReservationAsync(int id);
        Task<(bool Success, string ErrorMessage)> CancelReservationAsync(int id, string userId);
        Task<List<Order>> GetUserReservationsAsync(string userId);
        Task<List<dynamic>> GetAvailableTablesAsync(int peopleCount);
        bool ReservationExists(int id);
        
        // Phương thức hỗ trợ hiển thị dữ liệu
        Task<SelectList> GetTablesSelectListAsync(int? selectedTableId = null);
        Task<SelectList> GetCustomersSelectListAsync(string selectedCustomerId = null);
    }
} 