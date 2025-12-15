using RecipeSharingPlatform.Service.DTOs.ResponseDTOs; // Đảm bảo namespace này đúng với project của bạn

namespace RecipeSharingPlatform.Service.Services.Interfaces
{
    public interface ISavedRecipeService
    {
        // Trả về true nếu đã lưu, false nếu vừa bỏ lưu
        Task<bool> ToggleSaveRecipeAsync(int recipeId, int userId);

        // Kiểm tra xem user đã lưu recipe này chưa (dùng cho nút Save trên UI)
        Task<bool> IsRecipeSavedAsync(int recipeId, int userId);

        // Lấy danh sách để hiển thị trang Saved Recipes
        Task<List<RecipeListItemDTO>> GetSavedRecipesAsync(int userId);
    }
}