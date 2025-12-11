using System;
using System.Collections.Generic;

namespace RecipeSharingPlatform.Repository.Models;

public partial class RecipeImage
{
    public int ImageId { get; set; }

    public int RecipeId { get; set; }

    public string ImageUrl { get; set; } = null!;

    public int? ImageOrder { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Recipe Recipe { get; set; } = null!;
}
