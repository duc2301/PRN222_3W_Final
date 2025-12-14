using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RecipeSharingPlatform.Service.Services.Interfaces;
using System.Security.Claims;

namespace RecipeSharingPlatform.Presentation.Pages.Recipes
{
    public class DeleteModel : PageModel
    {
        private readonly IRecipeService _recipeService;

        public DeleteModel(IRecipeService recipeService)
        {
            _recipeService = recipeService;
        }

        // thông tin hiển thị nhanh trên trang confirm
        public int RecipeId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? FirstImageUrl { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr))
            {
                return RedirectToPage("/Auth/Login");
            }

            int userId = int.Parse(userIdStr);

            // Tận dụng GetRecipeForEditAsync để load nhanh info và cũng check owner
            var dto = await _recipeService.GetRecipeForEditAsync(id, userId);
            if (dto == null)
            {
                return NotFound();
            }

            RecipeId = dto.RecipeId;
            Title = dto.Title;
            if (dto.ImageUrls != null && dto.ImageUrls.Any())
            {
                FirstImageUrl = dto.ImageUrls.First();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr))
            {
                return RedirectToPage("/Auth/Login");
            }

            int userId = int.Parse(userIdStr);

            var success = await _recipeService.DeleteRecipeAsync(id, userId);
            if (!success)
            {
                // Không xoá được (không phải owner hoặc không tồn tại)
                return NotFound();
            }

            return RedirectToPage("Index");
        }
    }
}
