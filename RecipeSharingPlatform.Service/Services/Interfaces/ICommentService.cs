public interface ICommentService
{
    Task<List<CommentDto>> GetCommentsAsync(int recipeId);
    Task<CommentDto> AddCommentAsync(CommentCreateDto dto);
}