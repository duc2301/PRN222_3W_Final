using System;
using System.Collections.Generic;

namespace RecipeSharingPlatform.Repository.Models;

public partial class ShoppingListItem
{
    public int ItemId { get; set; }

    public int ShoppingListId { get; set; }

    public int? RecipeId { get; set; }

    public string IngredientName { get; set; } = null!;

    public decimal Quantity { get; set; }

    public string Unit { get; set; } = null!;

    public string? Category { get; set; }

    public bool? IsChecked { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Recipe? Recipe { get; set; }

    public virtual ShoppingList ShoppingList { get; set; } = null!;
}
