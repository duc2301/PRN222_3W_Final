using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RecipeSharingPlatform.Service.DTOs.RequestDTOs;
using RecipeSharingPlatform.Service.DTOs.ResponseDTOs;
using RecipeSharingPlatform.Service.ServiceProviders.Interface;
using System.Threading.Tasks;

namespace RecipeSharingPlatform.Presentation.Pages.User
{
    public class EditModel : PageModel
    {
        private readonly IServiceProviders _serviceProviders;
        private readonly IMapper _mapper;

        public EditModel(IServiceProviders serviceProviders, IMapper mapper)
        {
            _serviceProviders = serviceProviders;
            _mapper = mapper;
        }

        [BindProperty]
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
            User = user;
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var updatedUser = _mapper.Map<UpdateUserDTO>(User);
            await _serviceProviders.UserService.Update(updatedUser);

            return RedirectToPage("./Index");
        }
    }
}
