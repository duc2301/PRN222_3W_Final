using RecipeSharingPlatform.Repository.Models;

namespace RecipeSharingPlatform.Repository.Repositories.Interfaces
{
    public interface ISavedRecipeRepository
    {
        Task<SavedRecipe?> GetSavedRecipeAsync(int recipeId, int userId);
        Task<List<SavedRecipe>> GetSavedRecipesByUserIdAsync(int userId);
        Task AddAsync(SavedRecipe savedRecipe);
        Task RemoveAsync(SavedRecipe savedRecipe);
        Task SaveChangesAsync();
    }
}