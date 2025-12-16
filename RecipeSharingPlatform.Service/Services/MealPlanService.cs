using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RecipeSharingPlatform.Repository.DbContexts;
using RecipeSharingPlatform.Repository.Models;
using RecipeSharingPlatform.Repository.UnitOfWork.Interface;
using RecipeSharingPlatform.Service.DTOs.RequestDTOs;
using RecipeSharingPlatform.Service.DTOs.ResponseDTOs;
using RecipeSharingPlatform.Service.Services.Interfaces;
using System.Globalization;

namespace RecipeSharingPlatform.Service.Services
{
    public class MealPlanService : IMealPlanService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly RecipeSharingDbContext _context;
        private readonly IMapper _mapper;

        // Các loại bữa ăn cố định
        private static readonly List<(string Type, string Icon)> MealTypes = new()
        {
            ("Sáng", "🌅"),
            ("Trưa", "🌞"),
            ("Tối", "🌙"),
            ("Snack", "🍎")
        };

        // Tên các ngày trong tuần (tiếng Việt)
        private static readonly string[] DayNames = 
        {
            "Chủ Nhật", "Thứ Hai", "Thứ Ba", "Thứ Tư", "Thứ Năm", "Thứ Sáu", "Thứ Bảy"
        };

        public MealPlanService(IUnitOfWork unitOfWork, RecipeSharingDbContext context, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _context = context;
            _mapper = mapper;
        }

        public async Task<WeeklyMealPlanDTO> GetWeeklyMealPlanAsync(int userId, DateOnly? weekStartDate = null)
        {
            var startDate = weekStartDate ?? GetWeekStartDate(DateOnly.FromDateTime(DateTime.Today));
            var endDate = GetWeekEndDate(startDate);

            var mealPlans = await _unitOfWork.MealPlanRepository
                .GetWeeklyMealPlansAsync(userId, startDate, endDate);

            var result = new WeeklyMealPlanDTO
            {
                WeekStartDate = startDate,
                WeekEndDate = endDate,
                WeekLabel = $"{startDate:dd/MM} - {endDate:dd/MM/yyyy}",
                Days = new List<MealPlanDayDTO>()
            };

            var today = DateOnly.FromDateTime(DateTime.Today);

            // Tạo 7 ngày trong tuần
            for (int i = 0; i < 7; i++)
            {
                var currentDate = startDate.AddDays(i);
                var dayOfWeek = (int)currentDate.DayOfWeek;

                var dayDto = new MealPlanDayDTO
                {
                    Date = currentDate,
                    DayOfWeek = DayNames[dayOfWeek],
                    FormattedDate = currentDate.ToString("dd/MM/yyyy"),
                    IsToday = currentDate == today,
                    MealSlots = new List<MealSlotDTO>()
                };

                // Tạo 4 bữa ăn cho mỗi ngày
                foreach (var (mealType, icon) in MealTypes)
                {
                    var mealPlan = mealPlans
                        .FirstOrDefault(mp => mp.PlanDate == currentDate && mp.MealType == mealType);

                    var slot = new MealSlotDTO
                    {
                        MealPlanId = mealPlan?.MealPlanId,
                        MealType = mealType,
                        MealTypeIcon = icon,
                        Notes = mealPlan?.Notes,
                        Recipes = new List<MealPlanRecipeDTO>()
                    };

                    if (mealPlan?.MealPlanRecipes != null)
                    {
                        foreach (var mpr in mealPlan.MealPlanRecipes)
                        {
                            if (mpr.Recipe == null) continue;

                            slot.Recipes.Add(new MealPlanRecipeDTO
                            {
                                MealPlanRecipeId = mpr.MealPlanRecipeId,
                                RecipeId = mpr.RecipeId,
                                RecipeTitle = mpr.Recipe.Title,
                                MainImageUrl = mpr.Recipe.RecipeImages?
                                    .OrderBy(img => img.ImageOrder ?? 0)
                                    .FirstOrDefault()?.ImageUrl,
                                CategoryName = mpr.Recipe.Category?.CategoryName,
                                Difficulty = mpr.Recipe.Difficulty,
                                PrepTime = mpr.Recipe.PrepTime,
                                CookTime = mpr.Recipe.CookTime,
                                Servings = mpr.Servings ?? 4
                            });
                        }
                    }

                    dayDto.MealSlots.Add(slot);
                }

                result.Days.Add(dayDto);
            }

            return result;
        }

        public async Task<bool> AddRecipeToMealPlanAsync(int userId, AddRecipeToMealPlanDTO dto)
        {
            // Kiểm tra recipe tồn tại
            var recipe = await _unitOfWork.RecipeRepository.GetByIdAsync(dto.RecipeId);
            if (recipe == null)
            {
                return false;
            }

            // Tìm hoặc tạo meal plan cho ngày và bữa đó
            var mealPlan = await _unitOfWork.MealPlanRepository
                .GetMealPlanAsync(userId, dto.Date, dto.MealType);

            if (mealPlan == null)
            {
                mealPlan = new MealPlan
                {
                    UserId = userId,
                    PlanDate = dto.Date,
                    MealType = dto.MealType,
                    CreatedAt = DateTime.UtcNow
                };
                await _unitOfWork.MealPlanRepository.AddAsync(mealPlan);
                await _unitOfWork.SaveChangesAsync();
            }

            // Thêm recipe vào meal plan
            var mealPlanRecipe = new MealPlanRecipe
            {
                MealPlanId = mealPlan.MealPlanId,
                RecipeId = dto.RecipeId,
                Servings = dto.Servings
            };

            await _unitOfWork.MealPlanRepository.AddRecipeToMealPlanAsync(mealPlanRecipe);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> RemoveRecipeFromMealPlanAsync(int userId, int mealPlanRecipeId)
        {
            var result = await _unitOfWork.MealPlanRepository
                .RemoveRecipeFromMealPlanAsync(mealPlanRecipeId, userId);
            
            if (result)
            {
                await _unitOfWork.SaveChangesAsync();
            }

            return result;
        }

        public async Task<bool> UpdateNotesAsync(int userId, int mealPlanId, string? notes)
        {
            var result = await _unitOfWork.MealPlanRepository
                .UpdateNotesAsync(mealPlanId, userId, notes);
            
            if (result)
            {
                await _unitOfWork.SaveChangesAsync();
            }

            return result;
        }

        public async Task<int> GenerateShoppingListAsync(int userId, DateOnly weekStartDate)
        {
            var endDate = GetWeekEndDate(weekStartDate);
            
            // Lấy tất cả ingredients từ recipes trong tuần
            var ingredients = await _unitOfWork.MealPlanRepository
                .GetWeeklyIngredientsAsync(userId, weekStartDate, endDate);

            if (ingredients.Count == 0)
            {
                return 0;
            }

            // Lấy hoặc tạo shopping list của user
            var shoppingList = await _context.ShoppingLists
                .FirstOrDefaultAsync(sl => sl.UserId == userId);

            if (shoppingList == null)
            {
                shoppingList = new ShoppingList
                {
                    UserId = userId,
                    ListName = "My Shopping List",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                await _context.ShoppingLists.AddAsync(shoppingList);
                await _context.SaveChangesAsync();
            }

            // Load existing items
            var existingItems = await _context.ShoppingListItems
                .Where(i => i.ShoppingListId == shoppingList.ShoppingListId)
                .ToListAsync();

            int addedCount = 0;

            // Merge ingredients (group by name + unit)
            var groupedIngredients = ingredients
                .GroupBy(i => new { 
                    Name = i.IngredientName.Trim().ToLower(), 
                    Unit = (i.Unit ?? "").Trim().ToLower() 
                })
                .Select(g => new
                {
                    Name = g.First().IngredientName.Trim(),
                    Unit = g.First().Unit ?? "",
                    TotalQuantity = g.Sum(x => x.Quantity)
                })
                .ToList();

            foreach (var ing in groupedIngredients)
            {
                // Kiểm tra đã có item tương tự chưa
                var existing = existingItems.FirstOrDefault(e =>
                    e.IngredientName.Trim().ToLower() == ing.Name.ToLower() &&
                    (e.Unit ?? "").Trim().ToLower() == ing.Unit.ToLower());

                if (existing != null)
                {
                    // Cộng thêm số lượng
                    existing.Quantity += ing.TotalQuantity;
                }
                else
                {
                    // Thêm mới
                    var newItem = new ShoppingListItem
                    {
                        ShoppingListId = shoppingList.ShoppingListId,
                        IngredientName = ing.Name,
                        Quantity = ing.TotalQuantity,
                        Unit = ing.Unit,
                        Category = "Khác", // Default category
                        IsChecked = false,
                        CreatedAt = DateTime.UtcNow
                    };
                    await _context.ShoppingListItems.AddAsync(newItem);
                    addedCount++;
                }
            }

            shoppingList.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return addedCount;
        }

        public async Task<List<RecipeListItemDTO>> SearchRecipesForMealPlanAsync(string? search, int take = 20)
        {
            var query = _context.Recipes
                .Include(r => r.User)
                .Include(r => r.Category)
                .Include(r => r.RecipeImages)
                .Include(r => r.RecipeLikes)
                .Where(r => r.IsPublic == true)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim().ToLower();
                query = query.Where(r =>
                    r.Title.ToLower().Contains(search) ||
                    (r.Description != null && r.Description.ToLower().Contains(search)));
            }

            var recipes = await query
                .OrderByDescending(r => r.CreatedAt)
                .Take(take)
                .ToListAsync();

            return _mapper.Map<List<RecipeListItemDTO>>(recipes);
        }

        public DateOnly GetWeekStartDate(DateOnly date)
        {
            // Tính ngày Thứ 2 của tuần chứa date
            var dayOfWeek = (int)date.DayOfWeek;
            // Chủ nhật = 0, Thứ 2 = 1, ..., Thứ 7 = 6
            // Nếu là Chủ nhật (0), lùi 6 ngày. Ngược lại lùi (dayOfWeek - 1) ngày
            var daysToSubtract = dayOfWeek == 0 ? 6 : dayOfWeek - 1;
            return date.AddDays(-daysToSubtract);
        }

        public DateOnly GetWeekEndDate(DateOnly date)
        {
            // Chủ nhật là ngày cuối tuần
            var startDate = GetWeekStartDate(date);
            return startDate.AddDays(6);
        }
    }
}

