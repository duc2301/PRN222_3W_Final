using System;
using System.Collections.Generic;

namespace RecipeSharingPlatform.Repository.Models;

public partial class Recipe
{
    public int RecipeId { get; set; }

    public int UserId { get; set; }

    public int? CategoryId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public int? PrepTime { get; set; }

    public int? CookTime { get; set; }

    public int? Servings { get; set; }

    public string? Difficulty { get; set; }

    public bool? IsPublic { get; set; }

    public int? ViewCount { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Category? Category { get; set; }

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual ICollection<MealPlanRecipe> MealPlanRecipes { get; set; } = new List<MealPlanRecipe>();

    public virtual ICollection<RecipeImage> RecipeImages { get; set; } = new List<RecipeImage>();

    public virtual ICollection<RecipeIngredient> RecipeIngredients { get; set; } = new List<RecipeIngredient>();

    public virtual ICollection<RecipeLike> RecipeLikes { get; set; } = new List<RecipeLike>();

    public virtual ICollection<RecipeStep> RecipeSteps { get; set; } = new List<RecipeStep>();

    public virtual ICollection<SavedRecipe> SavedRecipes { get; set; } = new List<SavedRecipe>();

    public virtual ICollection<ShoppingListItem> ShoppingListItems { get; set; } = new List<ShoppingListItem>();

    public virtual User User { get; set; } = null!;
}
