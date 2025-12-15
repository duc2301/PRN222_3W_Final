using RecipeSharingPlatform.Repository.Models;

namespace RecipeSharingPlatform.Repository.Repositories.Interfaces
{
    public interface ILikeRepository
    {
        // Recipe Likes
        Task<RecipeLike?> GetRecipeLikeAsync(int recipeId, int userId);
        Task AddRecipeLikeAsync(RecipeLike like);
        Task RemoveRecipeLikeAsync(RecipeLike like);
        Task<int> GetRecipeLikeCountAsync(int recipeId);

        // Comment Likes
        Task<CommentLike?> GetCommentLikeAsync(int commentId, int userId);
        Task AddCommentLikeAsync(CommentLike like);
        Task RemoveCommentLikeAsync(CommentLike like);
        Task<int> GetCommentLikeCountAsync(int commentId);

        Task SaveChangesAsync();
    }
}