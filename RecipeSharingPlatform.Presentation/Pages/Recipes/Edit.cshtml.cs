using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using RecipeSharingPlatform.Service.DTOs.RequestDTOs;
using RecipeSharingPlatform.Service.Services.Interfaces;
using System.Security.Claims;

namespace RecipeSharingPlatform.Presentation.Pages.Recipes
{
    public class EditModel : PageModel
    {
        private readonly IRecipeService _recipeService;
        private readonly ICategoryService _categoryService;
        private readonly ICloudinaryService _cloudinaryService;

        public EditModel(
            IRecipeService recipeService,
            ICategoryService categoryService,
            ICloudinaryService cloudinaryService)
        {
            _recipeService = recipeService;
            _categoryService = categoryService;
            _cloudinaryService = cloudinaryService;
        }

        [BindProperty]
        public UpdateRecipeDTO Recipe { get; set; } = new UpdateRecipeDTO();

        // ảnh mới upload
        [BindProperty]
        public List<IFormFile> NewImages { get; set; } = new List<IFormFile>();

        // danh sách url ảnh cũ đang giữ (sẽ bind từ input hidden)
        [BindProperty]
        public List<string> ExistingImageUrls { get; set; } = new List<string>();

        public List<SelectListItem> CategoryOptions { get; set; } = new List<SelectListItem>();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr))
            {
                return RedirectToPage("/Auth/Login");
            }

            int userId = int.Parse(userIdStr);

            var dto = await _recipeService.GetRecipeForEditAsync(id, userId);
            if (dto == null)
            {
                return NotFound();
            }

            Recipe = dto;
            ExistingImageUrls = Recipe.ImageUrls ?? new List<string>();

            await LoadCategoriesAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr))
            {
                return RedirectToPage("/Auth/Login");
            }

            int userId = int.Parse(userIdStr);
            Recipe.UserId = userId;

            Recipe.Ingredients ??= new List<UpdateRecipeIngredientDTO>();
            Recipe.Steps ??= new List<UpdateRecipeStepDTO>();
            ExistingImageUrls ??= new List<string>();

            // 1) Upload ảnh mới, cộng dồn với ảnh cũ còn giữ
            var finalImageUrls = new List<string>();

            if (ExistingImageUrls.Count > 0)
            {
                finalImageUrls.AddRange(ExistingImageUrls);
            }

            if (NewImages != null && NewImages.Count > 0)
            {
                foreach (var file in NewImages)
                {
                    if (file == null || file.Length == 0) continue;

                    using var stream = file.OpenReadStream();
                    var uploadResult = await _cloudinaryService
                        .UploadImageAsync(stream, file.FileName, "recipes");

                    if (uploadResult != null && !string.IsNullOrEmpty(uploadResult.Url))
                    {
                        finalImageUrls.Add(uploadResult.Url);
                    }
                }
            }

            if (finalImageUrls.Count > 0)
            {
                Recipe.ImageUrls = finalImageUrls;
            }
            else
            {
                Recipe.ImageUrls = null;
            }

            if (!ModelState.IsValid)
            {
                await LoadCategoriesAsync();
                return Page();
            }

            var success = await _recipeService.UpdateRecipeAsync(Recipe);
            if (!success)
            {
                ModelState.AddModelError(string.Empty, "Không thể cập nhật công thức. Có thể bạn không phải chủ sở hữu.");
                await LoadCategoriesAsync();
                return Page();
            }

            return RedirectToPage("Details", new { id = Recipe.RecipeId });
        }

        private async Task LoadCategoriesAsync()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            CategoryOptions = categories
                .Select(c => new SelectListItem
                {
                    Value = c.CategoryId.ToString(),
                    Text = c.CategoryName
                })
                .ToList();
        }
    }
}
