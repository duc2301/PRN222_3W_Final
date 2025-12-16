using System.ComponentModel.DataAnnotations;

namespace RecipeSharingPlatform.Service.DTOs.RequestDTOs
{
    public class AddRecipeToMealPlanDTO
    {
        [Required]
        public DateOnly Date { get; set; }

        [Required]
        [RegularExpression("^(Sáng|Trưa|Tối|Snack)$", ErrorMessage = "MealType phải là: Sáng, Trưa, Tối hoặc Snack")]
        public string MealType { get; set; } = string.Empty;

        [Required]
        public int RecipeId { get; set; }

        [Range(1, 100)]
        public int Servings { get; set; } = 4;
    }
}

