using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RecipeSharingPlatform.Service.DTOs.ResponseDTOs;
using RecipeSharingPlatform.Service.Services.Interfaces; // Namespace chứa Service Interface

namespace RecipeSharingPlatform.Presentation.Pages.User
{
    public class ProfileModel : PageModel
    {
        private readonly IUserService _userService;

        public ProfileModel(IUserService userService)
        {
            _userService = userService;
        }

        public UserResponseDTO UserProfile { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            if (id <= 0)
            {
                return NotFound();
            }

            // Gọi service lấy thông tin chi tiết User theo ID
            UserProfile = await _userService.GetById(id);

            if (UserProfile == null)    
            {
                return NotFound();
            }

            return Page();
        }
    }
}