using RecipeSharingPlatform.Service.DTOs.RequestDTOs;
using RecipeSharingPlatform.Service.DTOs.ResponseDTOs;

namespace RecipeSharingPlatform.Service.Services.Interfaces
{
    public interface IMealPlanService
    {
        /// <summary>
        /// Lấy meal plan cho một tuần
        /// </summary>
        Task<WeeklyMealPlanDTO> GetWeeklyMealPlanAsync(int userId, DateOnly? weekStartDate = null);

        /// <summary>
        /// Thêm recipe vào meal plan
        /// </summary>
        Task<bool> AddRecipeToMealPlanAsync(int userId, AddRecipeToMealPlanDTO dto);

        /// <summary>
        /// Xóa recipe khỏi meal plan
        /// </summary>
        Task<bool> RemoveRecipeFromMealPlanAsync(int userId, int mealPlanRecipeId);

        /// <summary>
        /// Cập nhật notes cho meal plan
        /// </summary>
        Task<bool> UpdateNotesAsync(int userId, int mealPlanId, string? notes);

        /// <summary>
        /// Tạo shopping list từ tất cả recipes trong tuần
        /// </summary>
        Task<int> GenerateShoppingListAsync(int userId, DateOnly weekStartDate);

        /// <summary>
        /// Lấy danh sách recipes để search (dùng cho modal)
        /// </summary>
        Task<List<RecipeListItemDTO>> SearchRecipesForMealPlanAsync(string? search, int take = 20);

        /// <summary>
        /// Tính ngày đầu tuần (Thứ 2)
        /// </summary>
        DateOnly GetWeekStartDate(DateOnly date);

        /// <summary>
        /// Tính ngày cuối tuần (Chủ nhật)
        /// </summary>
        DateOnly GetWeekEndDate(DateOnly date);
    }
}

