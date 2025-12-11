using System;
using System.Collections.Generic;

namespace RecipeSharingPlatform.Repository.Models;

public partial class MealPlan
{
    public int MealPlanId { get; set; }

    public int UserId { get; set; }

    public DateOnly PlanDate { get; set; }

    public string? MealType { get; set; }

    public string? Notes { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<MealPlanRecipe> MealPlanRecipes { get; set; } = new List<MealPlanRecipe>();

    public virtual User User { get; set; } = null!;
}
