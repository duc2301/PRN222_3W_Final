using System;
using System.Collections.Generic;

namespace RecipeSharingPlatform.Repository.Models;

public partial class User
{
    public int UserId { get; set; }

    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? FullName { get; set; }

    public string? Bio { get; set; }

    public string? ProfileImage { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<CommentLike> CommentLikes { get; set; } = new List<CommentLike>();

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual ICollection<Follower> FollowerFollowerUsers { get; set; } = new List<Follower>();

    public virtual ICollection<Follower> FollowerFollowingUsers { get; set; } = new List<Follower>();

    public virtual ICollection<MealPlan> MealPlans { get; set; } = new List<MealPlan>();

    public virtual ICollection<RecipeLike> RecipeLikes { get; set; } = new List<RecipeLike>();

    public virtual ICollection<Recipe> Recipes { get; set; } = new List<Recipe>();

    public virtual ICollection<SavedRecipe> SavedRecipes { get; set; } = new List<SavedRecipe>();

    public virtual ICollection<ShoppingList> ShoppingLists { get; set; } = new List<ShoppingList>();
}
