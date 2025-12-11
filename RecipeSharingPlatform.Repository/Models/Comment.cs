using System;
using System.Collections.Generic;

namespace RecipeSharingPlatform.Repository.Models;

public partial class Comment
{
    public int CommentId { get; set; }

    public int RecipeId { get; set; }

    public int UserId { get; set; }

    public int? ParentCommentId { get; set; }

    public string CommentText { get; set; } = null!;

    public int? Rating { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<CommentLike> CommentLikes { get; set; } = new List<CommentLike>();

    public virtual ICollection<Comment> InverseParentComment { get; set; } = new List<Comment>();

    public virtual Comment? ParentComment { get; set; }

    public virtual Recipe Recipe { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
