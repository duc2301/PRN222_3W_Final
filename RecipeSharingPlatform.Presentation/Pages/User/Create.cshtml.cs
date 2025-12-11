using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RecipeSharingPlatform.Service.DTOs.RequestDTOs;
using RecipeSharingPlatform.Service.ServiceProviders.Interface;

namespace RecipeSharingPlatform.Presentation.Pages.User
{
    public class CreateModel : PageModel
    {
        private readonly IServiceProviders _serviceProviders;

        public CreateModel(IServiceProviders serviceProviders)
        {
            _serviceProviders = serviceProviders;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public CreateUserDTO User { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            await _serviceProviders.UserService.Create(User);

            return RedirectToPage("./Index");
        }
    }
}
