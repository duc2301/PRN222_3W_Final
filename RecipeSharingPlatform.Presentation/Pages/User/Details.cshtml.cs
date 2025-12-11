using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RecipeSharingPlatform.Service.DTOs.ResponseDTOs;
using RecipeSharingPlatform.Service.ServiceProviders.Interface;

namespace RecipeSharingPlatform.Presentation.Pages.User
{
    public class DetailsModel : PageModel
    {
        private readonly IServiceProviders _serviceProviders;

        public DetailsModel(IServiceProviders serviceProviders)
        {
            _serviceProviders = serviceProviders;
        }

        public UserResponseDTO User { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _serviceProviders.UserService.GetById(id);
            if (user == null)
            {
                return NotFound();
            }
            else
            {
                User = user;
            }
            return Page();
        }
    }
}
