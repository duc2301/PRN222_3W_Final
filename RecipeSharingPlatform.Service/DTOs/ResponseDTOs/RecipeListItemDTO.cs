using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeSharingPlatform.Service.DTOs.ResponseDTOs
{
    public class RecipeListItemDTO
    {
        public int RecipeId { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }

        public string AuthorName { get; set; } = null!;
        public string? AuthorAvatar { get; set; }

        public string? CategoryName { get; set; }

        public int? PrepTime { get; set; }
        public int? CookTime { get; set; }
        public int? Servings { get; set; }
        public string? Difficulty { get; set; }

        public int? ViewCount { get; set; }
        public int LikesCount { get; set; }

        public DateTime? CreatedAt { get; set; }

        public string? MainImageUrl { get; set; }
    }
}
