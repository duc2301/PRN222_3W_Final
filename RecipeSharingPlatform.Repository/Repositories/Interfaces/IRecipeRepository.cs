using RecipeSharingPlatform.Repository.Basic.Interfaces;
using RecipeSharingPlatform.Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeSharingPlatform.Repository.Repositories.Interfaces
{
    public interface IRecipeRepository : IGenericRepository<Recipe>
    {
        Task<(List<Recipe> Items, int TotalCount)> GetPagedRecipesAsync(
            string? search,
            int? categoryId,
            string? sortBy,
            int page,
            int pageSize);

        Task<List<Recipe>> GetByUserAsync(int userId, int? take = null);
        Task<Recipe?> GetDetailByIdAsync(int recipeId);
        Task AddAsync(Recipe recipe);
    }
}
