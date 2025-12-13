using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeSharingPlatform.Service.DTOs.RequestDTOs
{
    public class CreateRecipeRequestDTO
    {
        public int UserId { get; set; }

        public int? CategoryId { get; set; }

        public string Title { get; set; } = null!;

        public string? Description { get; set; }

        public int? PrepTime { get; set; }

        public int? CookTime { get; set; }

        public int? Servings { get; set; }

        public string? Difficulty { get; set; }

        public bool IsPublic { get; set; } = true;

        public List<CreateRecipeIngredientDTO> Ingredients { get; set; } = new();

        public List<CreateRecipeStepDTO> Steps { get; set; } = new();

        // các URL ảnh đã upload lên Cloudinary
        public List<string> ImageUrls { get; set; } = new();
    }
}
