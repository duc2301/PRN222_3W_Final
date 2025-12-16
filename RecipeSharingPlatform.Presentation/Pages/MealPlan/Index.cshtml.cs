using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RecipeSharingPlatform.Service.DTOs.RequestDTOs;
using RecipeSharingPlatform.Service.DTOs.ResponseDTOs;
using RecipeSharingPlatform.Service.Services.Interfaces;
using System.Security.Claims;

namespace RecipeSharingPlatform.Presentation.Pages.MealPlan
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly IMealPlanService _mealPlanService;

        public IndexModel(IMealPlanService mealPlanService)
        {
            _mealPlanService = mealPlanService;
        }

        public WeeklyMealPlanDTO WeeklyMealPlan { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string? WeekStart { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var userId = GetUserId();
            if (userId == null) return RedirectToPage("/Auth/Login");

            DateOnly? startDate = null;
            if (!string.IsNullOrEmpty(WeekStart) && DateOnly.TryParse(WeekStart, out var parsed))
            {
                startDate = parsed;
            }

            WeeklyMealPlan = await _mealPlanService.GetWeeklyMealPlanAsync(userId.Value, startDate);
            return Page();
        }

        // AJAX: Tìm kiếm recipes cho modal
        public async Task<IActionResult> OnGetSearchRecipesAsync(string? search)
        {
            var recipes = await _mealPlanService.SearchRecipesForMealPlanAsync(search, 20);
            return new JsonResult(new { ok = true, data = recipes });
        }

        // AJAX: Thêm recipe vào meal plan
        public async Task<IActionResult> OnPostAddRecipeAsync([FromBody] AddRecipeToMealPlanDTO dto)
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return new JsonResult(new { ok = false, message = "Vui lòng đăng nhập" });
            }

            if (dto == null || dto.RecipeId <= 0)
            {
                return new JsonResult(new { ok = false, message = "Dữ liệu không hợp lệ" });
            }

            try
            {
                var result = await _mealPlanService.AddRecipeToMealPlanAsync(userId.Value, dto);
                if (result)
                {
                    return new JsonResult(new { ok = true, message = "Đã thêm món ăn vào kế hoạch" });
                }
                return new JsonResult(new { ok = false, message = "Không tìm thấy món ăn" });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { ok = false, message = ex.Message });
            }
        }

        // AJAX: Xóa recipe khỏi meal plan
        public async Task<IActionResult> OnPostRemoveRecipeAsync([FromBody] RemoveRecipeRequest request)
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return new JsonResult(new { ok = false, message = "Vui lòng đăng nhập" });
            }

            try
            {
                var result = await _mealPlanService.RemoveRecipeFromMealPlanAsync(userId.Value, request.MealPlanRecipeId);
                if (result)
                {
                    return new JsonResult(new { ok = true, message = "Đã xóa món ăn khỏi kế hoạch" });
                }
                return new JsonResult(new { ok = false, message = "Không tìm thấy hoặc không có quyền xóa" });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { ok = false, message = ex.Message });
            }
        }

        // AJAX: Cập nhật notes
        public async Task<IActionResult> OnPostUpdateNotesAsync([FromBody] UpdateNotesRequest request)
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return new JsonResult(new { ok = false, message = "Vui lòng đăng nhập" });
            }

            try
            {
                var result = await _mealPlanService.UpdateNotesAsync(userId.Value, request.MealPlanId, request.Notes);
                if (result)
                {
                    return new JsonResult(new { ok = true, message = "Đã cập nhật ghi chú" });
                }
                return new JsonResult(new { ok = false, message = "Không thể cập nhật ghi chú" });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { ok = false, message = ex.Message });
            }
        }

        // AJAX: Tạo shopping list từ tuần
        public async Task<IActionResult> OnPostGenerateShoppingListAsync([FromBody] GenerateShoppingListRequest request)
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return new JsonResult(new { ok = false, message = "Vui lòng đăng nhập" });
            }

            try
            {
                if (!DateOnly.TryParse(request.WeekStart, out var weekStart))
                {
                    weekStart = _mealPlanService.GetWeekStartDate(DateOnly.FromDateTime(DateTime.Today));
                }

                var addedCount = await _mealPlanService.GenerateShoppingListAsync(userId.Value, weekStart);
                
                if (addedCount > 0)
                {
                    return new JsonResult(new { 
                        ok = true, 
                        message = $"Đã thêm {addedCount} nguyên liệu vào Shopping List",
                        redirectUrl = "/ShoppingList"
                    });
                }
                else
                {
                    return new JsonResult(new { 
                        ok = true, 
                        message = "Không có nguyên liệu mới để thêm (đã merge với các items hiện có)",
                        redirectUrl = "/ShoppingList"
                    });
                }
            }
            catch (Exception ex)
            {
                return new JsonResult(new { ok = false, message = ex.Message });
            }
        }

        private int? GetUserId()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(userIdStr, out var userId))
            {
                return userId;
            }
            return null;
        }

        // Request classes for AJAX
        public class RemoveRecipeRequest
        {
            public int MealPlanRecipeId { get; set; }
        }

        public class UpdateNotesRequest
        {
            public int MealPlanId { get; set; }
            public string? Notes { get; set; }
        }

        public class GenerateShoppingListRequest
        {
            public string? WeekStart { get; set; }
        }
    }
}

