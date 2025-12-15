using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RecipeSharingPlatform.Service.DTOs.RequestDTOs;
using RecipeSharingPlatform.Service.DTOs.ResponseDTOs;
using RecipeSharingPlatform.Service.Services.Interfaces;
using System.Security.Claims;

namespace RecipeSharingPlatform.Presentation.Pages.Recipes
{
    [IgnoreAntiforgeryToken]
    public class CommentsModel : PageModel
    {
        private readonly ICommentService _commentService;
        private readonly ILikeService _likeService; // Inject LikeService

        public CommentsModel(ICommentService commentService, ILikeService likeService)
        {
            _commentService = commentService;
            _likeService = likeService;
        }

        [BindProperty]
        public List<CommentDto> Comments { get; set; } = new();

        public async Task OnGetAsync(int recipeId)
        {
            Comments = await _commentService.GetCommentsAsync(recipeId);
            ViewData["RecipeId"] = recipeId;

            // --- Populate Like Info ---
            int? currentUserId = null;
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                var claimId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("UserId")?.Value;
                if (int.TryParse(claimId, out var id)) currentUserId = id;
            }

            foreach (var comment in Comments)
            {
                await PopulateLikeInfo(comment, currentUserId);
                if (comment.Replies != null)
                {
                    foreach (var reply in comment.Replies)
                    {
                        await PopulateLikeInfo(reply, currentUserId);
                    }
                }
            }
        }

        private async Task PopulateLikeInfo(CommentDto c, int? userId)
        {
            c.LikesCount = await _likeService.GetCommentLikeCountAsync(c.CommentId);
            if (userId.HasValue)
            {
                c.IsLiked = await _likeService.HasUserLikedCommentAsync(c.CommentId, userId.Value);
            }
        }

        public async Task<IActionResult> OnPostAddAsync([FromBody] CommentCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(new { error = "Invalid data" });
            try
            {
                var comment = await _commentService.AddCommentAsync(dto);
                return new JsonResult(comment);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // --- NEW: Toggle Like Handler ---
        public async Task<IActionResult> OnPostToggleLikeAsync(int commentId)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return new JsonResult(new { success = false, message = "Please login to like." });
            }

            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("UserId")?.Value;
            if (!int.TryParse(userIdStr, out var userId))
            {
                return new JsonResult(new { success = false, message = "User invalid." });
            }

            var result = await _likeService.ToggleCommentLikeAsync(commentId, userId);

            return new JsonResult(new
            {
                success = true,
                isLiked = result.IsLiked,
                newCount = result.NewCount
            });
        }
        public async Task<IActionResult> OnPostDeleteAsync(int commentId)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return new JsonResult(new { success = false, message = "Unauthorized" });
            }

            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("UserId")?.Value;
            if (!int.TryParse(userIdStr, out var userId))
            {
                return new JsonResult(new { success = false, message = "User invalid" });
            }

            var result = await _commentService.DeleteCommentAsync(commentId, userId);

            if (result)
            {
                return new JsonResult(new { success = true });
            }
            else
            {
                return new JsonResult(new { success = false, message = "Failed to delete (Not found or not owner)" });
            }
        }
    }
}