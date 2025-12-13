using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RecipeSharingPlatform.Service.DTOs.ResponseDTOs;
using RecipeSharingPlatform.Service.Services.Interfaces;
using System.Security.Claims;

namespace RecipeSharingPlatform.Presentation.Pages.Recipes
{
    public class DetailsModel : PageModel
    {
        private readonly IRecipeService _recipeService;

        public DetailsModel(IRecipeService recipeService)
        {
            _recipeService = recipeService;
        }

        public RecipeDetailDTO? Recipe { get; set; }
        public bool IsOwner { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            int? currentUserId = null;

            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                string? userIdValue =
                    User.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
                    User.FindFirst("UserId")?.Value;

                if (int.TryParse(userIdValue, out var parsed))
                {
                    currentUserId = parsed;
                }
            }

            Recipe = await _recipeService.GetRecipeDetailAsync(id, currentUserId);

            if (Recipe == null)
            {
                return NotFound();
            }

            IsOwner = currentUserId.HasValue && Recipe.UserId == currentUserId.Value;

            return Page();
        }
    }
}
