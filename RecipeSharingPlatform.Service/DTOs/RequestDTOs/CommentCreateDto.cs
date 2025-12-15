namespace RecipeSharingPlatform.Service.DTOs.RequestDTOs;
public class CommentCreateDto
{
    public int RecipeId { get; set; }
    public int UserId { get; set; }
    public string CommentText { get; set; } = string.Empty;
    public int? Rating { get; set; }
    public int? ParentCommentId { get; set; }
}