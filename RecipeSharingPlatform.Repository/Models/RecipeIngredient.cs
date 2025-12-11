using System;
using System.Collections.Generic;

namespace RecipeSharingPlatform.Repository.Models;

public partial class RecipeIngredient
{
    public int IngredientId { get; set; }

    public int RecipeId { get; set; }

    public string IngredientName { get; set; } = null!;

    public decimal Quantity { get; set; }

    public string Unit { get; set; } = null!;

    public int? OrderIndex { get; set; }

    public virtual Recipe Recipe { get; set; } = null!;
}
