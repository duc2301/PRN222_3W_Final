namespace RecipeSharingPlatform.Service.DTOs.ResponseDTOs
{
    public class CommentDto
    {
        public int CommentId { get; set; }
        public int? ParentCommentId { get; set; }
        public int RecipeId { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string AvatarUrl { get; set; } = string.Empty;
        public string CommentText { get; set; } = string.Empty;
        public int? Rating { get; set; }
        public DateTime? CreatedAt { get; set; }
        public List<CommentDto> Replies { get; set; } = new();

        // --- Add these lines ---
        public int LikesCount { get; set; }
        public bool IsLiked { get; set; }
    }
}