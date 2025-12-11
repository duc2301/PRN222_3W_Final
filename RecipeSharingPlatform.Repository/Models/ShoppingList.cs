using System;
using System.Collections.Generic;

namespace RecipeSharingPlatform.Repository.Models;

public partial class ShoppingList
{
    public int ShoppingListId { get; set; }

    public int UserId { get; set; }

    public string? ListName { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<ShoppingListItem> ShoppingListItems { get; set; } = new List<ShoppingListItem>();

    public virtual User User { get; set; } = null!;
}
