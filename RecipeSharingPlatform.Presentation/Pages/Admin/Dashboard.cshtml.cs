using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RecipeSharingPlatform.Service.DTOs.ResponseDTOs;
using RecipeSharingPlatform.Service.Services.Interfaces;

namespace RecipeSharingPlatform.Presentation.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class DashboardModel : PageModel
    {
        private readonly IDashboardService _dashboardService;

        public DashboardModel(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        public DashboardStatsDTO Stats { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            Stats = await _dashboardService.GetDashboardStatsAsync();
            return Page();
        }
    }
}

