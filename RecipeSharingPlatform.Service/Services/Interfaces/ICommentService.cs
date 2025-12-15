using RecipeSharingPlatform.Service.DTOs.RequestDTOs;
using RecipeSharingPlatform.Service.DTOs.ResponseDTOs;

namespace RecipeSharingPlatform.Service.Services.Interfaces;
public interface ICommentService
{
    Task<List<CommentDto>> GetCommentsAsync(int recipeId);
    Task<CommentDto> AddCommentAsync(CommentCreateDto dto);
    Task<bool> DeleteCommentAsync(int commentId, int userId);

}