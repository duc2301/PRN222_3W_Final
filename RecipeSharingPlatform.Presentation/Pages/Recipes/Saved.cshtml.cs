using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RecipeSharingPlatform.Service.DTOs.ResponseDTOs;
using RecipeSharingPlatform.Service.Services.Interfaces;
using System.Security.Claims;

namespace RecipeSharingPlatform.Presentation.Pages.Recipes
{
    [Authorize] // Require login to view saved recipes
    public class SavedModel : PageModel
    {
        private readonly ISavedRecipeService _savedRecipeService;

        public SavedModel(ISavedRecipeService savedRecipeService)
        {
            _savedRecipeService = savedRecipeService;
        }

        public List<RecipeListItemDTO> SavedRecipes { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            var userIdValue = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("UserId")?.Value;
            if (int.TryParse(userIdValue, out var userId))
            {
                SavedRecipes = await _savedRecipeService.GetSavedRecipesAsync(userId);
            }
            else
            {
                return RedirectToPage("/Auth/Login");
            }

            return Page();
        }
    }
}