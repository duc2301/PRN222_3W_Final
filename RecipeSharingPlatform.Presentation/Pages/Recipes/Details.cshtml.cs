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
        public List<CommentDto> Comments { get; set; } = new List<CommentDto>();

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

            // Map RecipeCommentDTO to CommentDto
            Comments = Recipe.Comments?
                .Select(c => MapRecipeCommentDtoToCommentDto(c))
                .ToList() ?? new List<CommentDto>();

            return Page();
        }

        private CommentDto MapRecipeCommentDtoToCommentDto(RecipeCommentDTO c)
        {
            return new CommentDto
            {
                CommentId = c.CommentId,
                RecipeId = c.RecipeId,
                UserId = c.UserId,
                Username = c.AuthorName,
                AvatarUrl = c.AuthorAvatar,
                CommentText = c.CommentText,
                Rating = null, // If you have a Rating property, map it here
                CreatedAt = c.CreatedAt,
                Replies = c.Replies?.Select(r => MapRecipeCommentDtoToCommentDto(r)).ToList() ?? new List<CommentDto>()
            };
        }
    }
}
