using RecipeSharingPlatform.Repository.Models;
using RecipeSharingPlatform.Repository.Repositories.Interfaces;
using RecipeSharingPlatform.Service.Services.Interfaces;
using System;
using System.Threading.Tasks;

namespace RecipeSharingPlatform.Service.Services
{
    public class LikeService : ILikeService
    {
        private readonly ILikeRepository _likeRepository;

        public LikeService(ILikeRepository likeRepository)
        {
            _likeRepository = likeRepository;
        }

        public async Task<(bool IsLiked, int NewCount)> ToggleRecipeLikeAsync(int recipeId, int userId)
        {
            // ... (keep existing logic) ...
            var existingLike = await _likeRepository.GetRecipeLikeAsync(recipeId, userId);
            bool isLiked;

            if (existingLike != null)
            {
                await _likeRepository.RemoveRecipeLikeAsync(existingLike);
                isLiked = false;
            }
            else
            {
                var newLike = new RecipeLike
                {
                    RecipeId = recipeId,
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow
                };
                await _likeRepository.AddRecipeLikeAsync(newLike);
                isLiked = true;
            }

            await _likeRepository.SaveChangesAsync();
            var newCount = await _likeRepository.GetRecipeLikeCountAsync(recipeId);

            return (isLiked, newCount);
        }

        public async Task<(bool IsLiked, int NewCount)> ToggleCommentLikeAsync(int commentId, int userId)
        {
           
            // Re-checking the previous LikeRepository code:
            // It had: Task<CommentLike?> GetCommentLikeAsync(int commentId, int userId);

            var like = await _likeRepository.GetCommentLikeAsync(commentId, userId);
            bool isLiked;

            if (like != null)
            {
                await _likeRepository.RemoveCommentLikeAsync(like);
                isLiked = false;
            }
            else
            {
                var newLike = new CommentLike
                {
                    CommentId = commentId,
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow
                };
                await _likeRepository.AddCommentLikeAsync(newLike);
                isLiked = true;
            }

            await _likeRepository.SaveChangesAsync();
            var newCount = await _likeRepository.GetCommentLikeCountAsync(commentId);

            return (isLiked, newCount);
        }

        public async Task<bool> HasUserLikedRecipeAsync(int recipeId, int userId)
        {
            var like = await _likeRepository.GetRecipeLikeAsync(recipeId, userId);
            return like != null;
        }

        public async Task<bool> HasUserLikedCommentAsync(int commentId, int userId)
        {
            var like = await _likeRepository.GetCommentLikeAsync(commentId, userId);
            return like != null;
        }

        // --- ADD THIS METHOD ---
        public async Task<int> GetCommentLikeCountAsync(int commentId)
        {
            return await _likeRepository.GetCommentLikeCountAsync(commentId);
        }
    }
}