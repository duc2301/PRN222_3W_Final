namespace RecipeSharingPlatform.Service.DTOs.ResponseDTOs
{
    public class MealSlotDTO
    {
        public int? MealPlanId { get; set; }
        public string MealType { get; set; } = string.Empty;
        public string MealTypeIcon { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public List<MealPlanRecipeDTO> Recipes { get; set; } = new();

        public bool HasRecipes => Recipes.Count > 0;
    }
}

