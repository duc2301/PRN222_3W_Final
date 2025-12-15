using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RecipeSharingPlatform.Service.DTOs;
using RecipeSharingPlatform.Service.Services.Interfaces;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace RecipeSharingPlatform.Presentation.Pages.Recipes
{
    [IgnoreAntiforgeryToken] // Add this if you want to skip CSRF validation
    // OR keep CSRF validation (recommended) - see below
    public class CommentsModel : PageModel
    {
        private readonly ICommentService _commentService;

        public CommentsModel(ICommentService commentService)
        {
            _commentService = commentService;
        }

        [BindProperty]
        public List<CommentDto> Comments { get; set; } = new();

        public async Task OnGetAsync(int recipeId)
        {
            Comments = await _commentService.GetCommentsAsync(recipeId);
            ViewData["RecipeId"] = recipeId;
        }

        [ValidateAntiForgeryToken] // Keep CSRF protection (recommended)
        public async Task<IActionResult> OnPostAddAsync([FromBody] CommentCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    error = "Invalid data",
                    errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                });
            }

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
    }
}