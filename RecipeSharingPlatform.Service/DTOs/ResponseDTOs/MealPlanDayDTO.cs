namespace RecipeSharingPlatform.Service.DTOs.ResponseDTOs
{
    public class MealPlanDayDTO
    {
        public DateOnly Date { get; set; }
        public string DayOfWeek { get; set; } = string.Empty;
        public string FormattedDate { get; set; } = string.Empty;
        public bool IsToday { get; set; }
        public List<MealSlotDTO> MealSlots { get; set; } = new();

        public bool HasAnyMeals => MealSlots.Any(ms => ms.HasRecipes);
    }
}

