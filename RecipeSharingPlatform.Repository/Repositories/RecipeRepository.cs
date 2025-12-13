using Microsoft.EntityFrameworkCore;
using RecipeSharingPlatform.Repository.Basic;
using RecipeSharingPlatform.Repository.DbContexts;
using RecipeSharingPlatform.Repository.Models;
using RecipeSharingPlatform.Repository.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeSharingPlatform.Repository.Repositories
{
    public class RecipeRepository : GenericRepository<Recipe>, IRecipeRepository
    {
        public RecipeRepository(RecipeSharingDbContext context) : base(context)
        {
        }

        public async Task<(List<Recipe> Items, int TotalCount)> GetPagedRecipesAsync(
            string? search,
            int? categoryId,
            string? sortBy,
            int page,
            int pageSize)
        {
            var query = _context.Recipes
                .Include(r => r.User)
                .Include(r => r.Category)
                .Include(r => r.RecipeImages)
                .Include(r => r.RecipeLikes)
                .AsQueryable();

            // Chỉ lấy recipe public (tạm thời)
            query = query.Where(r => r.IsPublic == true);

            // Search theo Title + Description
            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();
                query = query.Where(r =>
                    r.Title.Contains(search) ||
                    (r.Description != null && r.Description.Contains(search)));
            }

            // Filter theo Category
            if (categoryId.HasValue && categoryId.Value > 0)
            {
                query = query.Where(r => r.CategoryId == categoryId.Value);
            }

            // Sort
            query = sortBy switch
            {
                "mostViewed" => query.OrderByDescending(r => r.ViewCount)
                                     .ThenByDescending(r => r.CreatedAt),
                "mostLiked" => query.OrderByDescending(r => r.RecipeLikes.Count)
                                    .ThenByDescending(r => r.CreatedAt),
                _ => query.OrderByDescending(r => r.CreatedAt) // default: newest
            };

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        // NEW: Lấy recipe của 1 user
        public async Task<List<Recipe>> GetByUserAsync(int userId, int? take = null)
        {
            // ép kiểu rõ ràng là IQueryable<Recipe> từ đầu
            IQueryable<Recipe> query = _context.Recipes
                .Include(r => r.User)
                .Include(r => r.Category)
                .Include(r => r.RecipeImages)
                .Include(r => r.RecipeLikes)
                .Where(r => r.UserId == userId);

            // sắp xếp
            query = query.OrderByDescending(r => r.CreatedAt);

            // giới hạn số lượng nếu có truyền tham số
            if (take.HasValue && take.Value > 0)
            {
                query = query.Take(take.Value);
            }

            return await query.ToListAsync();
        }

        public async Task<Recipe?> GetDetailByIdAsync(int recipeId)
        {
            return await _context.Recipes
                .Include(r => r.User)
                .Include(r => r.Category)
                .Include(r => r.RecipeImages)
                .Include(r => r.RecipeIngredients)
                .Include(r => r.RecipeSteps)
                .Include(r => r.RecipeLikes)
                .Include(r => r.Comments)
                    .ThenInclude(c => c.User)
                .FirstOrDefaultAsync(r => r.RecipeId == recipeId);
        }

        public async Task AddAsync(Recipe recipe)
        {
            await _context.Recipes.AddAsync(recipe);
        }

    }
}
