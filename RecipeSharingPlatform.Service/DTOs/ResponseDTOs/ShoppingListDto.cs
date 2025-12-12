using System;

namespace RecipeSharingPlatform.Service.DTOs.ResponseDTOs
{
    public class ShoppingListDto
    {
        public int ShoppingListId { get; set; }

        public IReadOnlyList<ShoppingListItemDto> Items { get; set; } = Array.Empty<ShoppingListItemDto>();
    }
}

