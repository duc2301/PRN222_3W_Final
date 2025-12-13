using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeSharingPlatform.Service.DTOs.ResponseDTOs
{
    public class RecipeCommentDTO
    {
        public int CommentId { get; set; }
        public int UserId { get; set; }
        public int RecipeId { get; set; }

        public string CommentText { get; set; } = null!;
        public DateTime? CreatedAt { get; set; }

        public string AuthorName { get; set; } = null!;
        public string? AuthorAvatar { get; set; }

        public int? ParentCommentId { get; set; }
    }
}
