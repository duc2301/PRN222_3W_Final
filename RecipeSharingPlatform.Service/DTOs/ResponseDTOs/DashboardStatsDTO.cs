namespace RecipeSharingPlatform.Service.DTOs.ResponseDTOs
{
    /// <summary>
    /// Tổng hợp tất cả thống kê cho Admin Dashboard
    /// </summary>
    public class DashboardStatsDTO
    {
        // ========== TỔNG QUAN ==========
        public int TotalUsers { get; set; }
        public int TotalRecipes { get; set; }
        public int TotalComments { get; set; }
        public int TotalLikes { get; set; }
        public int TotalCategories { get; set; }
        public int TotalViews { get; set; }

        // ========== TĂNG TRƯỞNG SO VỚI THÁNG TRƯỚC ==========
        public double UserGrowthPercent { get; set; }
        public double RecipeGrowthPercent { get; set; }
        public double CommentGrowthPercent { get; set; }
        public double LikeGrowthPercent { get; set; }

        // ========== BIỂU ĐỒ: Công thức theo Category ==========
        public List<CategoryStatDTO> RecipesByCategory { get; set; } = new();

        // ========== BIỂU ĐỒ: Top 10 công thức xem nhiều nhất ==========
        public List<TopRecipeDTO> TopViewedRecipes { get; set; } = new();

        // ========== BIỂU ĐỒ: Top 10 công thức được like nhiều nhất ==========
        public List<TopRecipeDTO> TopLikedRecipes { get; set; } = new();

        // ========== BIỂU ĐỒ: Người dùng mới theo tháng (6 tháng gần nhất) ==========
        public List<MonthlyStatDTO> NewUsersMonthly { get; set; } = new();

        // ========== BIỂU ĐỒ: Công thức mới theo tháng ==========
        public List<MonthlyStatDTO> NewRecipesMonthly { get; set; } = new();

        // ========== BIỂU ĐỒ: Phân bố độ khó ==========
        public List<DifficultyStatDTO> RecipesByDifficulty { get; set; } = new();

        // ========== BIỂU ĐỒ: Top người dùng tích cực ==========
        public List<TopUserDTO> TopActiveUsers { get; set; } = new();

        // ========== BIỂU ĐỒ: Tỷ lệ Public vs Private ==========
        public int PublicRecipes { get; set; }
        public int PrivateRecipes { get; set; }

        // ========== BIỂU ĐỒ: Rating trung bình theo Category ==========
        public List<CategoryRatingDTO> RatingsByCategory { get; set; } = new();

        // ========== DANH SÁCH: Công thức gần đây ==========
        public List<RecentRecipeDTO> RecentRecipes { get; set; } = new();

        // ========== DANH SÁCH: Bình luận gần đây ==========
        public List<RecentCommentDTO> RecentComments { get; set; } = new();

        // ========== DANH SÁCH: Người dùng mới ==========
        public List<RecentUserDTO> RecentUsers { get; set; } = new();
    }

    public class CategoryStatDTO
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public int RecipeCount { get; set; }
        public string Color { get; set; } = string.Empty;
    }

    public class TopRecipeDTO
    {
        public int RecipeId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? AuthorName { get; set; }
        public int ViewCount { get; set; }
        public int LikeCount { get; set; }
        public string? ImageUrl { get; set; }
    }

    public class MonthlyStatDTO
    {
        public string MonthLabel { get; set; } = string.Empty;
        public int Year { get; set; }
        public int Month { get; set; }
        public int Count { get; set; }
    }

    public class DifficultyStatDTO
    {
        public string Difficulty { get; set; } = string.Empty;
        public int Count { get; set; }
        public string Color { get; set; } = string.Empty;
    }

    public class TopUserDTO
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string? FullName { get; set; }
        public string? ProfileImage { get; set; }
        public int RecipeCount { get; set; }
        public int TotalLikes { get; set; }
        public int FollowerCount { get; set; }
    }

    public class CategoryRatingDTO
    {
        public string CategoryName { get; set; } = string.Empty;
        public double AverageRating { get; set; }
    }

    public class RecentRecipeDTO
    {
        public int RecipeId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? AuthorName { get; set; }
        public string? CategoryName { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? ImageUrl { get; set; }
    }

    public class RecentCommentDTO
    {
        public int CommentId { get; set; }
        public string CommentText { get; set; } = string.Empty;
        public string? Username { get; set; }
        public string? RecipeTitle { get; set; }
        public int? Rating { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class RecentUserDTO
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string? FullName { get; set; }
        public string Email { get; set; } = string.Empty;
        public string? ProfileImage { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

