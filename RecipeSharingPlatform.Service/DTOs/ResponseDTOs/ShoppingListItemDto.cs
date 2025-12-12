namespace RecipeSharingPlatform.Service.DTOs.ResponseDTOs
{
    public class ShoppingListItemDto
    {
        public int ItemId { get; set; }
        public string Name { get; set; } = null!;
        public decimal Quantity { get; set; }
        public string Unit { get; set; } = null!;
        public string Category { get; set; } = "Khác";
        public bool? IsChecked { get; set; }
        public int? RecipeId { get; set; }
    }
}

