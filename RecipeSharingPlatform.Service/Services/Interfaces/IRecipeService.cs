using RecipeSharingPlatform.Service.DTOs.RequestDTOs;
using RecipeSharingPlatform.Service.DTOs.ResponseDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeSharingPlatform.Service.Services.Interfaces
{
    public interface IRecipeService
    {
        Task<PagedResultDTO<RecipeListItemDTO>> GetRecipesAsync(RecipeListFilterDTO filter);
        // Các method khác: GetDetail, Create, Update, Delete sẽ thêm ở bước sau

        Task<List<RecipeListItemDTO>> GetUserRecipesAsync(int userId, int? take = null);

        Task<RecipeDetailDTO?> GetRecipeDetailAsync(int recipeId, int? currentUserId);
        Task<int> CreateRecipeAsync(CreateRecipeRequestDTO request);
        Task<UpdateRecipeDTO?> GetRecipeForEditAsync(int recipeId, int currentUserId);
        Task<bool> UpdateRecipeAsync(UpdateRecipeDTO request);
        Task<bool> DeleteRecipeAsync(int recipeId, int currentUserId);
        Task<bool> IncreaseViewCountAsync(int recipeId);
    }
}
