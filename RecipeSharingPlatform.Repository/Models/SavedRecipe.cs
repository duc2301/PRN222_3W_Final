using System;
using System.Collections.Generic;

namespace RecipeSharingPlatform.Repository.Models;

public partial class SavedRecipe
{
    public int SaveId { get; set; }

    public int RecipeId { get; set; }

    public int UserId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Recipe Recipe { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
