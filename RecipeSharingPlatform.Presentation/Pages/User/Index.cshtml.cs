using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RecipeSharingPlatform.Service.DTOs.ResponseDTOs;
using RecipeSharingPlatform.Service.ServiceProviders.Interface;

namespace RecipeSharingPlatform.Presentation.Pages.User
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly IServiceProviders _serviceProviders;

        public IndexModel(IServiceProviders serviceProviders)
        {
            _serviceProviders = serviceProviders;
        }

        public IList<UserResponseDTO> User { get; set; } = default!;

        public async Task OnGetAsync()
        {
            User = await _serviceProviders.UserService.GetAll();
        }
    }
}
