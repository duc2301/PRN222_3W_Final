using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using RecipeSharingPlatform.Service.DTOs.RequestDTOs;
using RecipeSharingPlatform.Service.Services.Interfaces;
using System.Security.Claims;

namespace RecipeSharingPlatform.Presentation.Pages.Recipes
{
    public class CreateModel : PageModel
    {
        private readonly IRecipeService _recipeService;
        private readonly ICategoryService _categoryService;
        private readonly ICloudinaryService _cloudinaryService;

        public CreateModel(
            IRecipeService recipeService,
            ICategoryService categoryService,
            ICloudinaryService cloudinaryService)
        {
            _recipeService = recipeService;
            _categoryService = categoryService;
            _cloudinaryService = cloudinaryService;
        }

        public class IngredientInput
        {
            public string IngredientName { get; set; } = string.Empty;
            public decimal? Quantity { get; set; }
            public string? Unit { get; set; }
        }

        public class StepInput
        {
            public string Description { get; set; } = string.Empty;
        }

        [BindProperty]
        public string Title { get; set; } = string.Empty;

        [BindProperty]
        public string? Description { get; set; }

        [BindProperty]
        public int? CategoryId { get; set; }

        [BindProperty]
        public int? PrepTime { get; set; }

        [BindProperty]
        public int? CookTime { get; set; }

        [BindProperty]
        public int? Servings { get; set; }

        [BindProperty]
        public string? Difficulty { get; set; }

        [BindProperty]
        public bool IsPublic { get; set; } = true;

        [BindProperty]
        public List<IngredientInput> Ingredients { get; set; } = new();

        [BindProperty]
        public List<StepInput> Steps { get; set; } = new();

        [BindProperty]
        public List<IFormFile> RecipeImages { get; set; } = new();

        public List<SelectListItem> CategoryOptions { get; set; } = new();

        public async Task OnGetAsync()
        {
            await LoadCategoriesAsync();
        }

        private async Task LoadCategoriesAsync()
        {
            var categories = await _categoryService.GetAllCategoriesAsync(); // đổi theo DTO bạn đang dùng

            CategoryOptions = categories
                .Select(c => new SelectListItem
                {
                    Value = c.CategoryId.ToString(),
                    Text = c.CategoryName
                })
                .ToList();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadCategoriesAsync();
                return Page();
            }

            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr))
            {
                return Challenge();
            }

            int userId = int.Parse(userIdStr);

            var imageUrls = new List<string>();

            if (RecipeImages != null && RecipeImages.Count > 0)
            {
                foreach (var file in RecipeImages)
                {
                    if (file != null && file.Length > 0)
                    {
                        using (var stream = file.OpenReadStream())
                        {
                            var result = await _cloudinaryService
                                .UploadImageAsync(stream, file.FileName, "recipes");

                            // Đổi "Url" sang "SecureUrl" nếu DTO của bạn đặt tên khác
                            if (result != null && !string.IsNullOrEmpty(result.Url))
                            {
                                imageUrls.Add(result.Url);
                            }
                        }
                    }
                }
            }


            var ingredientDtos = new List<CreateRecipeIngredientDTO>();
            int ingIndex = 0;
            foreach (var ing in Ingredients.Where(i => !string.IsNullOrWhiteSpace(i.IngredientName)))
            {
                ingredientDtos.Add(new CreateRecipeIngredientDTO
                {
                    IngredientName = ing.IngredientName.Trim(),
                    Quantity = ing.Quantity ?? 0,
                    Unit = ing.Unit ?? string.Empty,
                    OrderIndex = ingIndex++
                });
            }

            var stepDtos = new List<CreateRecipeStepDTO>();
            int stepNumber = 1;
            foreach (var step in Steps.Where(s => !string.IsNullOrWhiteSpace(s.Description)))
            {
                stepDtos.Add(new CreateRecipeStepDTO
                {
                    StepNumber = stepNumber++,
                    StepDescription = step.Description.Trim(),
                    ImageUrl = null
                });
            }

            var request = new CreateRecipeRequestDTO
            {
                UserId = userId,
                CategoryId = CategoryId,
                Title = Title,
                Description = Description,
                PrepTime = PrepTime,
                CookTime = CookTime,
                Servings = Servings,
                Difficulty = Difficulty,
                IsPublic = IsPublic,
                ImageUrls = imageUrls,
                Ingredients = ingredientDtos,
                Steps = stepDtos
            };

            var newRecipeId = await _recipeService.CreateRecipeAsync(request);

            return RedirectToPage("/Recipes/Details", new { id = newRecipeId });
        }   
    }
}
