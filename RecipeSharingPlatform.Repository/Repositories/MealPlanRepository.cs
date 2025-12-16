using Microsoft.EntityFrameworkCore;
using RecipeSharingPlatform.Repository.Basic;
using RecipeSharingPlatform.Repository.DbContexts;
using RecipeSharingPlatform.Repository.Models;
using RecipeSharingPlatform.Repository.Repositories.Interfaces;

namespace RecipeSharingPlatform.Repository.Repositories
{
    public class MealPlanRepository : GenericRepository<MealPlan>, IMealPlanRepository
    {
        public MealPlanRepository(RecipeSharingDbContext context) : base(context)
        {
        }

        public async Task<List<MealPlan>> GetWeeklyMealPlansAsync(int userId, DateOnly startDate, DateOnly endDate)
        {
            return await _context.MealPlans
                .Include(mp => mp.MealPlanRecipes)
                    .ThenInclude(mpr => mpr.Recipe)
                        .ThenInclude(r => r.RecipeImages)
                .Include(mp => mp.MealPlanRecipes)
                    .ThenInclude(mpr => mpr.Recipe)
                        .ThenInclude(r => r.Category)
                .Where(mp => mp.UserId == userId 
                    && mp.PlanDate >= startDate 
                    && mp.PlanDate <= endDate)
                .OrderBy(mp => mp.PlanDate)
                .ThenBy(mp => mp.MealType)
                .ToListAsync();
        }

        public async Task<MealPlan?> GetMealPlanAsync(int userId, DateOnly date, string mealType)
        {
            return await _context.MealPlans
                .Include(mp => mp.MealPlanRecipes)
                    .ThenInclude(mpr => mpr.Recipe)
                .FirstOrDefaultAsync(mp => 
                    mp.UserId == userId 
                    && mp.PlanDate == date 
                    && mp.MealType == mealType);
        }

        public async Task<MealPlan?> GetMealPlanWithRecipesAsync(int mealPlanId)
        {
            return await _context.MealPlans
                .Include(mp => mp.MealPlanRecipes)
                    .ThenInclude(mpr => mpr.Recipe)
                        .ThenInclude(r => r.RecipeImages)
                .Include(mp => mp.MealPlanRecipes)
                    .ThenInclude(mpr => mpr.Recipe)
                        .ThenInclude(r => r.Category)
                .FirstOrDefaultAsync(mp => mp.MealPlanId == mealPlanId);
        }

        public async Task AddAsync(MealPlan mealPlan)
        {
            await _context.MealPlans.AddAsync(mealPlan);
        }

        public async Task AddRecipeToMealPlanAsync(MealPlanRecipe mealPlanRecipe)
        {
            await _context.MealPlanRecipes.AddAsync(mealPlanRecipe);
        }

        public async Task<bool> RemoveRecipeFromMealPlanAsync(int mealPlanRecipeId, int userId)
        {
            var mealPlanRecipe = await _context.MealPlanRecipes
                .Include(mpr => mpr.MealPlan)
                .FirstOrDefaultAsync(mpr => 
                    mpr.MealPlanRecipeId == mealPlanRecipeId 
                    && mpr.MealPlan.UserId == userId);

            if (mealPlanRecipe == null)
            {
                return false;
            }

            _context.MealPlanRecipes.Remove(mealPlanRecipe);
            return true;
        }

        public async Task<bool> UpdateNotesAsync(int mealPlanId, int userId, string? notes)
        {
            var mealPlan = await _context.MealPlans
                .FirstOrDefaultAsync(mp => mp.MealPlanId == mealPlanId && mp.UserId == userId);

            if (mealPlan == null)
            {
                return false;
            }

            mealPlan.Notes = notes;
            return true;
        }

        public async Task<List<RecipeIngredient>> GetWeeklyIngredientsAsync(int userId, DateOnly startDate, DateOnly endDate)
        {
            var mealPlanRecipes = await _context.MealPlanRecipes
                .Include(mpr => mpr.MealPlan)
                .Include(mpr => mpr.Recipe)
                    .ThenInclude(r => r.RecipeIngredients)
                .Where(mpr => 
                    mpr.MealPlan.UserId == userId 
                    && mpr.MealPlan.PlanDate >= startDate 
                    && mpr.MealPlan.PlanDate <= endDate)
                .ToListAsync();

            var ingredients = new List<RecipeIngredient>();

            foreach (var mpr in mealPlanRecipes)
            {
                if (mpr.Recipe?.RecipeIngredients == null) continue;

                var baseServings = mpr.Recipe.Servings ?? 4;
                var targetServings = mpr.Servings ?? 4;
                var ratio = (decimal)targetServings / baseServings;

                foreach (var ing in mpr.Recipe.RecipeIngredients)
                {
                    // Tạo bản sao với số lượng đã scale
                    ingredients.Add(new RecipeIngredient
                    {
                        IngredientName = ing.IngredientName,
                        Quantity = decimal.Round(ing.Quantity * ratio, 2, MidpointRounding.AwayFromZero),
                        Unit = ing.Unit,
                        RecipeId = ing.RecipeId
                    });
                }
            }

            return ingredients;
        }
    }
}

