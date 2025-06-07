using RestaurantManagement.Models.Dashboard;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RestaurantManagement.Services.Interfaces
{
    public interface IDashboardService
    {
        Task<DashboardData> GetDashboardDataAsync();
    }
} 