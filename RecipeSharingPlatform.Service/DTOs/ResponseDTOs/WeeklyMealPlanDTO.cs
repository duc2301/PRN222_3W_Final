namespace RecipeSharingPlatform.Service.DTOs.ResponseDTOs
{
    public class WeeklyMealPlanDTO
    {
        public DateOnly WeekStartDate { get; set; }
        public DateOnly WeekEndDate { get; set; }
        public string WeekLabel { get; set; } = string.Empty;
        public List<MealPlanDayDTO> Days { get; set; } = new();

        public int TotalRecipes => Days.Sum(d => d.MealSlots.Sum(ms => ms.Recipes.Count));
    }
}

