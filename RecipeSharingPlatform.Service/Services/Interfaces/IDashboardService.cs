using RecipeSharingPlatform.Service.DTOs.ResponseDTOs;

namespace RecipeSharingPlatform.Service.Services.Interfaces
{
    public interface IDashboardService
    {
        /// <summary>
        /// Lấy tất cả thống kê cho Admin Dashboard
        /// </summary>
        Task<DashboardStatsDTO> GetDashboardStatsAsync();
    }
}

