using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RecipeSharingPlatform.Repository.Models;
using RecipeSharingPlatform.Service.DTOs.ResponseDTOs;
using RecipeSharingPlatform.Service.Services;
using RecipeSharingPlatform.Service.Services.Interfaces;
using System.Security.Claims;

namespace RecipeSharingPlatform.Presentation.Pages.Recipes
{
    public class DetailsModel : PageModel
    {
        private readonly IRecipeService _recipeService;
        private readonly ILikeService _likeService; // 1. Inject LikeService
        private readonly ISavedRecipeService _savedRecipeService;

        public DetailsModel(IRecipeService recipeService, ILikeService likeService, ISavedRecipeService savedRecipeService) 
        {
            _recipeService = recipeService;
            _likeService = likeService;
            _savedRecipeService = savedRecipeService;
        }
        public bool IsSaved { get; set; }

        public RecipeDetailDTO? Recipe { get; set; }
        public bool IsOwner { get; set; }
        public bool IsLiked { get; set; } // 2. Trạng thái Like của user hiện tại
        public List<CommentDto> Comments { get; set; } = new List<CommentDto>();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            int? currentUserId = null;

            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                string? userIdValue = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("UserId")?.Value;
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

            // 3. Kiểm tra xem user đã like chưa
            if (currentUserId.HasValue)
            {
                IsLiked = await _likeService.HasUserLikedRecipeAsync(id, currentUserId.Value);
                IsSaved = await _savedRecipeService.IsRecipeSavedAsync(id, currentUserId.Value);
            }

            // Map comments
            Comments = Recipe.Comments?
                .Select(c => MapRecipeCommentDtoToCommentDto(c))
                .ToList() ?? new List<CommentDto>();

            return Page();
        }

        // 4. AJAX Handler để Toggle Like
        public async Task<IActionResult> OnPostToggleLikeAsync(int id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return new JsonResult(new { success = false, message = "Please login to like." });
            }

            var userIdValue = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("UserId")?.Value;
            if (!int.TryParse(userIdValue, out var userId))
            {
                return new JsonResult(new { success = false, message = "User invalid." });
            }

            // Gọi service toggle
            var result = await _likeService.ToggleRecipeLikeAsync(id, userId);

            return new JsonResult(new
            {
                success = true,
                isLiked = result.IsLiked,
                newCount = result.NewCount
            });
        }
        public async Task<IActionResult> OnPostToggleSaveAsync(int id)
        {
            if (!User.Identity.IsAuthenticated)
                return new JsonResult(new { success = false, message = "Please login to save." });

            var userIdValue = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("UserId")?.Value;
            if (!int.TryParse(userIdValue, out var userId))
                return new JsonResult(new { success = false, message = "User invalid." });

            var isSaved = await _savedRecipeService.ToggleSaveRecipeAsync(id, userId);

            return new JsonResult(new { success = true, isSaved = isSaved });
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
                Rating = null,
                CreatedAt = c.CreatedAt,
                Replies = c.Replies?.Select(r => MapRecipeCommentDtoToCommentDto(r)).ToList() ?? new List<CommentDto>()
            };
        }
    }
}