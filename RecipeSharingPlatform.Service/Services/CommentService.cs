using RecipeSharingPlatform.Repository.Models;
using RecipeSharingPlatform.Repository.Repositories.Interfaces;
using RecipeSharingPlatform.Service.DTOs.RequestDTOs;
using RecipeSharingPlatform.Service.DTOs.ResponseDTOs;
using RecipeSharingPlatform.Service.Services.Interfaces;
namespace RecipeSharingPlatform.Service.Services;
public class CommentService : ICommentService
{
    private readonly ICommentRepository _commentRepository;
    private readonly IUserRepository _userRepository;

    public CommentService(ICommentRepository commentRepository, IUserRepository userRepository)
    {
        _commentRepository = commentRepository;
        _userRepository = userRepository;
    }

    public async Task<List<CommentDto>> GetCommentsAsync(int recipeId)
    {
        var comments = await _commentRepository.GetCommentsByRecipeIdAsync(recipeId);
        return comments.Select(MapToDto).ToList();
    }

    public async Task<CommentDto> AddCommentAsync(CommentCreateDto dto)
    {
        var user = await _userRepository.GetByIdAsync(dto.UserId);
        var comment = new Comment
        {
            RecipeId = dto.RecipeId,
            UserId = dto.UserId,
            CommentText = dto.CommentText,
            Rating = dto.Rating,
            ParentCommentId = dto.ParentCommentId,
            CreatedAt = DateTime.UtcNow
        };
        await _commentRepository.AddAsync(comment);
        await _commentRepository.SaveChangesAsync();

        // Reload with user info
        comment.User = user;
        return MapToDto(comment);
    }
    public async Task<bool> DeleteCommentAsync(int commentId, int userId)
    {
        var comment = await _commentRepository.GetByIdAsync(commentId);

        if (comment == null) return false;

        // Check if the user requesting delete is the owner
        if (comment.UserId != userId) return false;

        _commentRepository.Delete(comment);
        await _commentRepository.SaveChangesAsync();
        return true;
    }
    private CommentDto MapToDto(Comment comment)
    {
        return new CommentDto
        {
            CommentId = comment.CommentId,
            RecipeId = comment.RecipeId,
            UserId = comment.UserId,
            Username = comment.User?.Username ?? "",
            AvatarUrl = comment.User?.ProfileImage ?? "",
            CommentText = comment.CommentText,
            Rating = comment.Rating,
            CreatedAt = comment.CreatedAt,
            Replies = comment.InverseParentComment?.Select(MapToDto).ToList() ?? new()
        };
    }
}