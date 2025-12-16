namespace RecipeSharingPlatform.Service.DTOs.ResponseDTOs
{
    public class MealPlanRecipeDTO
    {
        public int MealPlanRecipeId { get; set; }
        public int RecipeId { get; set; }
        public string RecipeTitle { get; set; } = string.Empty;
        public string? MainImageUrl { get; set; }
        public string? CategoryName { get; set; }
        public string? Difficulty { get; set; }
        public int? PrepTime { get; set; }
        public int? CookTime { get; set; }
        public int Servings { get; set; }
    }
}

