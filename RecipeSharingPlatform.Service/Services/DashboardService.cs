using Microsoft.EntityFrameworkCore;
using RecipeSharingPlatform.Repository.DbContexts;
using RecipeSharingPlatform.Service.DTOs.ResponseDTOs;
using RecipeSharingPlatform.Service.Services.Interfaces;

namespace RecipeSharingPlatform.Service.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly RecipeSharingDbContext _context;

        // Màu sắc cho biểu đồ
        private static readonly string[] ChartColors = new[]
        {
            "#FF6384", "#36A2EB", "#FFCE56", "#4BC0C0", "#9966FF",
            "#FF9F40", "#FF6384", "#C9CBCF", "#7BC225", "#E7E9ED"
        };

        private static readonly Dictionary<string, string> DifficultyColors = new()
        {
            { "Dễ", "#4BC0C0" },
            { "Trung bình", "#FFCE56" },
            { "Khó", "#FF6384" }
        };

        public DashboardService(RecipeSharingDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardStatsDTO> GetDashboardStatsAsync()
        {
            var now = DateTime.UtcNow;
            var startOfMonth = new DateTime(now.Year, now.Month, 1);
            var startOfLastMonth = startOfMonth.AddMonths(-1);

            var stats = new DashboardStatsDTO();

            // ========== TỔNG QUAN ==========
            stats.TotalUsers = await _context.Users.CountAsync(u => u.IsActive == true);
            stats.TotalRecipes = await _context.Recipes.CountAsync();
            stats.TotalComments = await _context.Comments.CountAsync();
            stats.TotalLikes = await _context.RecipeLikes.CountAsync();
            stats.TotalCategories = await _context.Categories.CountAsync();
            stats.TotalViews = await _context.Recipes.SumAsync(r => r.ViewCount ?? 0);

            // ========== TĂNG TRƯỞNG SO VỚI THÁNG TRƯỚC ==========
            var usersThisMonth = await _context.Users.CountAsync(u => u.CreatedAt >= startOfMonth);
            var usersLastMonth = await _context.Users.CountAsync(u => u.CreatedAt >= startOfLastMonth && u.CreatedAt < startOfMonth);
            stats.UserGrowthPercent = CalculateGrowth(usersLastMonth, usersThisMonth);

            var recipesThisMonth = await _context.Recipes.CountAsync(r => r.CreatedAt >= startOfMonth);
            var recipesLastMonth = await _context.Recipes.CountAsync(r => r.CreatedAt >= startOfLastMonth && r.CreatedAt < startOfMonth);
            stats.RecipeGrowthPercent = CalculateGrowth(recipesLastMonth, recipesThisMonth);

            var commentsThisMonth = await _context.Comments.CountAsync(c => c.CreatedAt >= startOfMonth);
            var commentsLastMonth = await _context.Comments.CountAsync(c => c.CreatedAt >= startOfLastMonth && c.CreatedAt < startOfMonth);
            stats.CommentGrowthPercent = CalculateGrowth(commentsLastMonth, commentsThisMonth);

            var likesThisMonth = await _context.RecipeLikes.CountAsync(l => l.CreatedAt >= startOfMonth);
            var likesLastMonth = await _context.RecipeLikes.CountAsync(l => l.CreatedAt >= startOfLastMonth && l.CreatedAt < startOfMonth);
            stats.LikeGrowthPercent = CalculateGrowth(likesLastMonth, likesThisMonth);

            // ========== CÔNG THỨC THEO CATEGORY ==========
            var categoryStats = await _context.Recipes
                .Where(r => r.CategoryId != null)
                .GroupBy(r => new { r.CategoryId, r.Category!.CategoryName })
                .Select(g => new { g.Key.CategoryId, g.Key.CategoryName, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .ToListAsync();

            stats.RecipesByCategory = categoryStats.Select((c, index) => new CategoryStatDTO
            {
                CategoryId = c.CategoryId ?? 0,
                CategoryName = c.CategoryName ?? "Khác",
                RecipeCount = c.Count,
                Color = ChartColors[index % ChartColors.Length]
            }).ToList();

            // ========== TOP 10 CÔNG THỨC XEM NHIỀU NHẤT ==========
            stats.TopViewedRecipes = await _context.Recipes
                .Include(r => r.User)
                .Include(r => r.RecipeImages)
                .Include(r => r.RecipeLikes)
                .OrderByDescending(r => r.ViewCount)
                .Take(10)
                .Select(r => new TopRecipeDTO
                {
                    RecipeId = r.RecipeId,
                    Title = r.Title,
                    AuthorName = r.User.FullName ?? r.User.Username,
                    ViewCount = r.ViewCount ?? 0,
                    LikeCount = r.RecipeLikes.Count,
                    ImageUrl = r.RecipeImages.OrderBy(i => i.ImageOrder).Select(i => i.ImageUrl).FirstOrDefault()
                })
                .ToListAsync();

            // ========== TOP 10 CÔNG THỨC ĐƯỢC LIKE NHIỀU NHẤT ==========
            stats.TopLikedRecipes = await _context.Recipes
                .Include(r => r.User)
                .Include(r => r.RecipeImages)
                .Include(r => r.RecipeLikes)
                .OrderByDescending(r => r.RecipeLikes.Count)
                .Take(10)
                .Select(r => new TopRecipeDTO
                {
                    RecipeId = r.RecipeId,
                    Title = r.Title,
                    AuthorName = r.User.FullName ?? r.User.Username,
                    ViewCount = r.ViewCount ?? 0,
                    LikeCount = r.RecipeLikes.Count,
                    ImageUrl = r.RecipeImages.OrderBy(i => i.ImageOrder).Select(i => i.ImageUrl).FirstOrDefault()
                })
                .ToListAsync();

            // ========== NGƯỜI DÙNG MỚI THEO THÁNG (6 tháng gần nhất) ==========
            var sixMonthsAgo = now.AddMonths(-6);
            var usersByMonth = await _context.Users
                .Where(u => u.CreatedAt >= sixMonthsAgo)
                .GroupBy(u => new { u.CreatedAt!.Value.Year, u.CreatedAt.Value.Month })
                .Select(g => new { g.Key.Year, g.Key.Month, Count = g.Count() })
                .OrderBy(x => x.Year).ThenBy(x => x.Month)
                .ToListAsync();

            stats.NewUsersMonthly = usersByMonth.Select(u => new MonthlyStatDTO
            {
                Year = u.Year,
                Month = u.Month,
                MonthLabel = $"T{u.Month}/{u.Year}",
                Count = u.Count
            }).ToList();

            // ========== CÔNG THỨC MỚI THEO THÁNG ==========
            var recipesByMonth = await _context.Recipes
                .Where(r => r.CreatedAt >= sixMonthsAgo)
                .GroupBy(r => new { r.CreatedAt!.Value.Year, r.CreatedAt.Value.Month })
                .Select(g => new { g.Key.Year, g.Key.Month, Count = g.Count() })
                .OrderBy(x => x.Year).ThenBy(x => x.Month)
                .ToListAsync();

            stats.NewRecipesMonthly = recipesByMonth.Select(r => new MonthlyStatDTO
            {
                Year = r.Year,
                Month = r.Month,
                MonthLabel = $"T{r.Month}/{r.Year}",
                Count = r.Count
            }).ToList();

            // ========== PHÂN BỐ ĐỘ KHÓ ==========
            var difficultyStats = await _context.Recipes
                .Where(r => r.Difficulty != null)
                .GroupBy(r => r.Difficulty)
                .Select(g => new { Difficulty = g.Key, Count = g.Count() })
                .ToListAsync();

            stats.RecipesByDifficulty = difficultyStats.Select(d => new DifficultyStatDTO
            {
                Difficulty = d.Difficulty ?? "Khác",
                Count = d.Count,
                Color = DifficultyColors.GetValueOrDefault(d.Difficulty ?? "", "#C9CBCF")
            }).ToList();

            // ========== TOP NGƯỜI DÙNG TÍCH CỰC ==========
            stats.TopActiveUsers = await _context.Users
                .Where(u => u.IsActive == true && u.Role != "Admin")
                .Select(u => new TopUserDTO
                {
                    UserId = u.UserId,
                    Username = u.Username,
                    FullName = u.FullName,
                    ProfileImage = u.ProfileImage,
                    RecipeCount = u.Recipes.Count,
                    TotalLikes = u.Recipes.SelectMany(r => r.RecipeLikes).Count(),
                    FollowerCount = u.FollowerFollowingUsers.Count
                })
                .OrderByDescending(u => u.RecipeCount + u.TotalLikes)
                .Take(5)
                .ToListAsync();

            // ========== TỶ LỆ PUBLIC VS PRIVATE ==========
            stats.PublicRecipes = await _context.Recipes.CountAsync(r => r.IsPublic == true);
            stats.PrivateRecipes = await _context.Recipes.CountAsync(r => r.IsPublic == false);

            // ========== RATING TRUNG BÌNH THEO CATEGORY ==========
            var ratingsByCategory = await _context.Comments
                .Where(c => c.Rating != null && c.Recipe.CategoryId != null)
                .GroupBy(c => c.Recipe.Category!.CategoryName)
                .Select(g => new CategoryRatingDTO
                {
                    CategoryName = g.Key ?? "Khác",
                    AverageRating = g.Average(c => c.Rating ?? 0)
                })
                .ToListAsync();

            stats.RatingsByCategory = ratingsByCategory;

            // ========== CÔNG THỨC GẦN ĐÂY ==========
            stats.RecentRecipes = await _context.Recipes
                .Include(r => r.User)
                .Include(r => r.Category)
                .Include(r => r.RecipeImages)
                .OrderByDescending(r => r.CreatedAt)
                .Take(5)
                .Select(r => new RecentRecipeDTO
                {
                    RecipeId = r.RecipeId,
                    Title = r.Title,
                    AuthorName = r.User.FullName ?? r.User.Username,
                    CategoryName = r.Category != null ? r.Category.CategoryName : null,
                    CreatedAt = r.CreatedAt ?? DateTime.MinValue,
                    ImageUrl = r.RecipeImages.OrderBy(i => i.ImageOrder).Select(i => i.ImageUrl).FirstOrDefault()
                })
                .ToListAsync();

            // ========== BÌNH LUẬN GẦN ĐÂY ==========
            stats.RecentComments = await _context.Comments
                .Include(c => c.User)
                .Include(c => c.Recipe)
                .OrderByDescending(c => c.CreatedAt)
                .Take(5)
                .Select(c => new RecentCommentDTO
                {
                    CommentId = c.CommentId,
                    CommentText = c.CommentText,
                    Username = c.User.Username,
                    RecipeTitle = c.Recipe.Title,
                    Rating = c.Rating,
                    CreatedAt = c.CreatedAt ?? DateTime.MinValue
                })
                .ToListAsync();

            // ========== NGƯỜI DÙNG MỚI ==========
            stats.RecentUsers = await _context.Users
                .Where(u => u.Role != "Admin")
                .OrderByDescending(u => u.CreatedAt)
                .Take(5)
                .Select(u => new RecentUserDTO
                {
                    UserId = u.UserId,
                    Username = u.Username,
                    FullName = u.FullName,
                    Email = u.Email,
                    ProfileImage = u.ProfileImage,
                    CreatedAt = u.CreatedAt ?? DateTime.MinValue
                })
                .ToListAsync();

            return stats;
        }

        private static double CalculateGrowth(int previous, int current)
        {
            if (previous == 0) return current > 0 ? 100 : 0;
            return Math.Round((double)(current - previous) / previous * 100, 1);
        }
    }
}

