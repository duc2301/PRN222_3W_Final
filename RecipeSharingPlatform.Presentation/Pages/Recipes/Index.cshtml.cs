using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RecipeSharingPlatform.Service.DTOs.RequestDTOs;
using RecipeSharingPlatform.Service.DTOs.ResponseDTOs;
using RecipeSharingPlatform.Service.Services.Interfaces;
using System.Diagnostics;
using System.Security.Claims;

namespace RecipeSharingPlatform.Presentation.Pages.Recipes
{
    public class IndexModel : PageModel
    {
        private readonly IRecipeService _recipeService;
        private readonly ICategoryService _categoryService;

        public IndexModel(IRecipeService recipeService, ICategoryService categoryService)
        {
            _recipeService = recipeService;
            _categoryService = categoryService;
        }

        // Filters (binding từ query string)
        [BindProperty(SupportsGet = true)]
        public string? Search { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? CategoryId { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? SortBy { get; set; } = "newest";

        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;


        public PagedResultDTO<RecipeListItemDTO> Recipes { get; set; } = new();
        public List<CategoryResponseDTO> Categories { get; set; } = new();
        public List<RecipeListItemDTO> MyRecipes { get; set; } = new();


        public async Task OnGetAsync()
        {
            Categories = await _categoryService.GetAllCategoriesAsync();

            var filter = new RecipeListFilterDTO
            {
                Search = Search,
                CategoryId = CategoryId,
                SortBy = SortBy,
                Page = PageNumber,
                PageSize = 12
            };

            Recipes = await _recipeService.GetRecipesAsync(filter);

            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (int.TryParse(userIdClaim, out var userId))
                {
                    MyRecipes = await _recipeService.GetUserRecipesAsync(userId, 6);
                }
            }
        }
    }
}
