using Microsoft.EntityFrameworkCore;
using RecipeSharingPlatform.Repository.DbContexts;
using RecipeSharingPlatform.Repository.Models;
using RecipeSharingPlatform.Repository.Repositories.Interfaces;

namespace RecipeSharingPlatform.Repository.Repositories
{
    public class LikeRepository : ILikeRepository
    {
        private readonly RecipeSharingDbContext _context;

        public LikeRepository(RecipeSharingDbContext context)
        {
            _context = context;
        }

        #region Recipe Likes
        public async Task<RecipeLike?> GetRecipeLikeAsync(int recipeId, int userId)
        {
            return await _context.RecipeLikes
                .FirstOrDefaultAsync(l => l.RecipeId == recipeId && l.UserId == userId);
        }

        public async Task AddRecipeLikeAsync(RecipeLike like)
        {
            await _context.RecipeLikes.AddAsync(like);
        }

        public async Task RemoveRecipeLikeAsync(RecipeLike like)
        {
            _context.RecipeLikes.Remove(like);
            await Task.CompletedTask;
        }

        public async Task<int> GetRecipeLikeCountAsync(int recipeId)
        {
            return await _context.RecipeLikes.CountAsync(l => l.RecipeId == recipeId);
        }
        #endregion

        #region Comment Likes
        public async Task<CommentLike?> GetCommentLikeAsync(int commentId, int userId)
        {
            return await _context.CommentLikes
                .FirstOrDefaultAsync(l => l.CommentId == commentId && l.UserId == userId);
        }

        public async Task AddCommentLikeAsync(CommentLike like)
        {
            await _context.CommentLikes.AddAsync(like);
        }

        public async Task RemoveCommentLikeAsync(CommentLike like)
        {
            _context.CommentLikes.Remove(like);
            await Task.CompletedTask;
        }

        public async Task<int> GetCommentLikeCountAsync(int commentId)
        {
            return await _context.CommentLikes.CountAsync(l => l.CommentId == commentId);
        }
        #endregion

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}