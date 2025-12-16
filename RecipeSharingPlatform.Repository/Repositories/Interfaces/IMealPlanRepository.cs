using RecipeSharingPlatform.Repository.Basic.Interfaces;
using RecipeSharingPlatform.Repository.Models;

namespace RecipeSharingPlatform.Repository.Repositories.Interfaces
{
    public interface IMealPlanRepository : IGenericRepository<MealPlan>
    {
        /// <summary>
        /// Lấy tất cả meal plans trong một tuần cho user
        /// </summary>
        Task<List<MealPlan>> GetWeeklyMealPlansAsync(int userId, DateOnly startDate, DateOnly endDate);

        /// <summary>
        /// Lấy meal plan theo ngày và bữa ăn
        /// </summary>
        Task<MealPlan?> GetMealPlanAsync(int userId, DateOnly date, string mealType);

        /// <summary>
        /// Lấy meal plan với chi tiết recipes
        /// </summary>
        Task<MealPlan?> GetMealPlanWithRecipesAsync(int mealPlanId);

        /// <summary>
        /// Thêm meal plan mới
        /// </summary>
        Task AddAsync(MealPlan mealPlan);

        /// <summary>
        /// Thêm recipe vào meal plan
        /// </summary>
        Task AddRecipeToMealPlanAsync(MealPlanRecipe mealPlanRecipe);

        /// <summary>
        /// Xóa recipe khỏi meal plan
        /// </summary>
        Task<bool> RemoveRecipeFromMealPlanAsync(int mealPlanRecipeId, int userId);

        /// <summary>
        /// Cập nhật notes cho meal plan
        /// </summary>
        Task<bool> UpdateNotesAsync(int mealPlanId, int userId, string? notes);

        /// <summary>
        /// Lấy tất cả ingredients từ các recipes trong tuần để tạo shopping list
        /// </summary>
        Task<List<RecipeIngredient>> GetWeeklyIngredientsAsync(int userId, DateOnly startDate, DateOnly endDate);
    }
}

