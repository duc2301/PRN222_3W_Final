using System;
using System.Collections.Generic;

namespace RecipeSharingPlatform.Repository.Models;

public partial class MealPlanRecipe
{
    public int MealPlanRecipeId { get; set; }

    public int MealPlanId { get; set; }

    public int RecipeId { get; set; }

    public int? Servings { get; set; }

    public virtual MealPlan MealPlan { get; set; } = null!;

    public virtual Recipe Recipe { get; set; } = null!;
}
