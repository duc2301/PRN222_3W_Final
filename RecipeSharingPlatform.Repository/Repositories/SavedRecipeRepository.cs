using Microsoft.EntityFrameworkCore;
using RecipeSharingPlatform.Repository.DbContexts;
using RecipeSharingPlatform.Repository.Models;
using RecipeSharingPlatform.Repository.Repositories.Interfaces;

namespace RecipeSharingPlatform.Repository.Repositories
{
    public class SavedRecipeRepository : ISavedRecipeRepository
    {
        private readonly RecipeSharingDbContext _context;

        public SavedRecipeRepository(RecipeSharingDbContext context)
        {
            _context = context;
        }

        public async Task<SavedRecipe?> GetSavedRecipeAsync(int recipeId, int userId)
        {
            return await _context.SavedRecipes
                .FirstOrDefaultAsync(sr => sr.RecipeId == recipeId && sr.UserId == userId);
        }

        public async Task<List<SavedRecipe>> GetSavedRecipesByUserIdAsync(int userId)
        {
            return await _context.SavedRecipes
                .Include(sr => sr.Recipe)
                    .ThenInclude(r => r.Category)
                .Include(sr => sr.Recipe)
                    .ThenInclude(r => r.User) // Tác giả công thức
                .Include(sr => sr.Recipe)
                    .ThenInclude(r => r.RecipeImages)
                .Include(sr => sr.Recipe)
                    .ThenInclude(r => r.RecipeLikes)
                .Where(sr => sr.UserId == userId)
                .OrderByDescending(sr => sr.CreatedAt)
                .ToListAsync();
        }

        public async Task AddAsync(SavedRecipe savedRecipe)
        {
            await _context.SavedRecipes.AddAsync(savedRecipe);
        }

        public async Task RemoveAsync(SavedRecipe savedRecipe)
        {
            _context.SavedRecipes.Remove(savedRecipe);
            await Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}