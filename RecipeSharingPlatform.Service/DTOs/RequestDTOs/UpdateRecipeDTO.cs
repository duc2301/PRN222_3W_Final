using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeSharingPlatform.Service.DTOs.RequestDTOs
{
    public class UpdateRecipeIngredientDTO
    {
        public string IngredientName { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public string Unit { get; set; } = string.Empty;
    }

    public class UpdateRecipeStepDTO
    {
        public int StepNumber { get; set; }   // dùng cho hiển thị, service sẽ tự đánh lại
        public string StepDescription { get; set; } = string.Empty;
    }

    public class UpdateRecipeDTO
    {
        [Required]
        public int RecipeId { get; set; }

        // set trong PageModel từ Claims, không bind từ view
        public int UserId { get; set; }

        public int? CategoryId { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [StringLength(2000)]
        public string? Description { get; set; }

        public int? PrepTime { get; set; }
        public int? CookTime { get; set; }
        public int? Servings { get; set; }
        public string? Difficulty { get; set; }

        // true = public, false = private
        public bool IsPublic { get; set; } = true;

        // danh sách nguyên liệu
        public List<UpdateRecipeIngredientDTO> Ingredients { get; set; }
            = new List<UpdateRecipeIngredientDTO>();

        // danh sách bước
        public List<UpdateRecipeStepDTO> Steps { get; set; }
            = new List<UpdateRecipeStepDTO>();

        // danh sách ảnh mới (nếu có) – nếu null hoặc rỗng thì giữ nguyên ảnh cũ
        public List<string>? ImageUrls { get; set; }
    }
}
