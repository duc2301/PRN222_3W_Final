using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RecipeSharingPlatform.Service.DTOs.RequestDTOs;
using RecipeSharingPlatform.Service.DTOs.ResponseDTOs;
using RecipeSharingPlatform.Service.Services.Interfaces;

namespace RecipeSharingPlatform.Presentation.Pages.ShoppingList
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly IShoppingListService _shoppingListService;

        public IndexModel(IShoppingListService shoppingListService)
        {
            _shoppingListService = shoppingListService;
        }

        public IReadOnlyList<GroupedItemsDto> GroupedItems { get; set; } = new List<GroupedItemsDto>();

        public async Task OnGetAsync()
        {
            GroupedItems = await _shoppingListService.GetGroupedItemsAsync(GetUserId());
        }

        public async Task<JsonResult> OnPostAddRecipeAsync([FromBody] AddRecipePayload payload)
        {
            if (payload == null)
            {
                return Error("Payload trống");
            }

            try
            {
                await _shoppingListService.AddRecipeToListAsync(GetUserId(), payload.RecipeId, payload.TargetServings);
                return Success("Đã thêm vào shopping list");
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        public async Task<JsonResult> OnPostAddManualAsync([FromForm] AddItemDto dto)
        {
            if (!ModelState.IsValid)
            {
                return Error("Dữ liệu không hợp lệ.");
            }

            try
            {
                await _shoppingListService.AddManualItemAsync(GetUserId(), dto);
                return Success("Đã thêm item");
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        public async Task<JsonResult> OnPostToggleItemAsync([FromForm] int itemId, [FromForm] bool isChecked)
        {
            try
            {
                await _shoppingListService.ToggleItemAsync(GetUserId(), itemId, isChecked);
                return Success("Đã cập nhật");
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        public async Task<JsonResult> OnPostDeleteItemAsync([FromForm] int itemId)
        {
            try
            {
                await _shoppingListService.DeleteItemAsync(GetUserId(), itemId);
                return Success("Đã xóa item");
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        public async Task<JsonResult> OnPostClearCompletedAsync()
        {
            try
            {
                await _shoppingListService.ClearCompletedAsync(GetUserId());
                return Success("Đã xóa các mục hoàn thành");
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        private string GetUserId()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                throw new InvalidOperationException("Không tìm thấy thông tin người dùng.");
            }

            return userId;
        }

        private JsonResult Success(string message, object? data = null)
        {
            return new JsonResult(new { ok = true, message, data });
        }

        private JsonResult Error(string message)
        {
            return new JsonResult(new { ok = false, message });
        }

        public class AddRecipePayload
        {
            public int RecipeId { get; set; }
            public int TargetServings { get; set; }
        }
    }
}

