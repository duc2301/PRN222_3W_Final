using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RecipeSharingPlatform.Repository.DbContexts;
using RecipeSharingPlatform.Repository.Models;
using RecipeSharingPlatform.Service.DTOs.ResponseDTOs;
using RecipeSharingPlatform.Service.ServiceProviders.Interface;

namespace RecipeSharingPlatform.Presentation.Pages.User
{
    public class DeleteModel : PageModel
    {
        private readonly IServiceProviders _serviceProviders;

        public DeleteModel(IServiceProviders serviceProviders)
        {
            _serviceProviders = serviceProviders;
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
            else
            {
                User = user;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _serviceProviders.UserService.Delete(id);

            return RedirectToPage("./Index");
        }
    }
}
