using System;

namespace RecipeSharingPlatform.Service.DTOs.ResponseDTOs
{
    public class GroupedItemsDto
    {
        public string Category { get; set; } = "Khác";

        public IReadOnlyList<ShoppingListItemDto> Items { get; set; } = Array.Empty<ShoppingListItemDto>();
    }
}

