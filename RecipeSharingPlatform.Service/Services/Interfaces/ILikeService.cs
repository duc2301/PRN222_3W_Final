namespace RecipeSharingPlatform.Service.Services.Interfaces
{
    public interface ILikeService
    {
        // Trả về (IsLiked: trạng thái mới, NewCount: số lượng like mới)
        Task<(bool IsLiked, int NewCount)> ToggleRecipeLikeAsync(int recipeId, int userId);
        Task<(bool IsLiked, int NewCount)> ToggleCommentLikeAsync(int commentId, int userId);

        Task<bool> HasUserLikedRecipeAsync(int recipeId, int userId);
        Task<bool> HasUserLikedCommentAsync(int commentId, int userId);
        Task<int> GetCommentLikeCountAsync(int commentId);
    }
}