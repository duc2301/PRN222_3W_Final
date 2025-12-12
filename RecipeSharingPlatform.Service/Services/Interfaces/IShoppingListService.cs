using RecipeSharingPlatform.Service.DTOs.RequestDTOs;
using RecipeSharingPlatform.Service.DTOs.ResponseDTOs;

namespace RecipeSharingPlatform.Service.Services.Interfaces
{
    public interface IShoppingListService
    {
        Task<ShoppingListDto> GetOrCreateAsync(string userId);

        Task AddRecipeToListAsync(string userId, int recipeId, int targetServings);

        Task AddManualItemAsync(string userId, AddItemDto dto);

        Task ToggleItemAsync(string userId, int itemId, bool isChecked);

        Task DeleteItemAsync(string userId, int itemId);

        Task ClearCompletedAsync(string userId);

        Task<IReadOnlyList<GroupedItemsDto>> GetGroupedItemsAsync(string userId);
    }
}

