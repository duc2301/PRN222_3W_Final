using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using RecipeSharingPlatform.Repository.Models;

namespace RecipeSharingPlatform.Repository.DbContexts;

public partial class RecipeSharingDbContext : DbContext
{
    public RecipeSharingDbContext()
    {
    }

    public RecipeSharingDbContext(DbContextOptions<RecipeSharingDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Comment> Comments { get; set; }

    public virtual DbSet<CommentLike> CommentLikes { get; set; }

    public virtual DbSet<Follower> Followers { get; set; }

    public virtual DbSet<MealPlan> MealPlans { get; set; }

    public virtual DbSet<MealPlanRecipe> MealPlanRecipes { get; set; }

    public virtual DbSet<Recipe> Recipes { get; set; }

    public virtual DbSet<RecipeImage> RecipeImages { get; set; }

    public virtual DbSet<RecipeIngredient> RecipeIngredients { get; set; }

    public virtual DbSet<RecipeLike> RecipeLikes { get; set; }

    public virtual DbSet<RecipeStep> RecipeSteps { get; set; }

    public virtual DbSet<SavedRecipe> SavedRecipes { get; set; }

    public virtual DbSet<ShoppingList> ShoppingLists { get; set; }

    public virtual DbSet<ShoppingListItem> ShoppingListItems { get; set; }

    public virtual DbSet<User> Users { get; set; }

//    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
//        => optionsBuilder.UseSqlServer("server=(local); database=RecipeSharingDB; uid=sa; pwd=12345; TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__Categori__19093A2B871CEFC5");

            entity.HasIndex(e => e.CategoryName, "UQ__Categori__8517B2E08CD69599").IsUnique();

            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.CategoryName).HasMaxLength(50);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Description).HasMaxLength(255);
        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => e.CommentId).HasName("PK__Comments__C3B4DFAAFCC17F22");

            entity.HasIndex(e => e.ParentCommentId, "IX_Comments_ParentCommentID");

            entity.HasIndex(e => e.RecipeId, "IX_Comments_RecipeID");

            entity.HasIndex(e => e.UserId, "IX_Comments_UserID");

            entity.Property(e => e.CommentId).HasColumnName("CommentID");
            entity.Property(e => e.CommentText).HasMaxLength(1000);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.ParentCommentId).HasColumnName("ParentCommentID");
            entity.Property(e => e.RecipeId).HasColumnName("RecipeID");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.ParentComment).WithMany(p => p.InverseParentComment)
                .HasForeignKey(d => d.ParentCommentId)
                .HasConstraintName("FK_Comments_ParentComment")
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(d => d.Recipe).WithMany(p => p.Comments)
                .HasForeignKey(d => d.RecipeId)
                .HasConstraintName("FK_Comments_Recipes");

            entity.HasOne(d => d.User).WithMany(p => p.Comments)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Comments_Users");
        });

        modelBuilder.Entity<CommentLike>(entity =>
        {
            entity.HasKey(e => e.CommentLikeId).HasName("PK__CommentL__D36E159DED551C92");

            entity.HasIndex(e => e.CommentId, "IX_CommentLikes_CommentID");

            entity.HasIndex(e => e.UserId, "IX_CommentLikes_UserID");

            entity.HasIndex(e => new { e.CommentId, e.UserId }, "UQ_CommentLikes_CommentUser").IsUnique();

            entity.Property(e => e.CommentLikeId).HasColumnName("CommentLikeID");
            entity.Property(e => e.CommentId).HasColumnName("CommentID");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Comment).WithMany(p => p.CommentLikes)
                .HasForeignKey(d => d.CommentId)
                .HasConstraintName("FK_CommentLikes_Comments");

            entity.HasOne(d => d.User).WithMany(p => p.CommentLikes)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CommentLikes_Users");
        });

        modelBuilder.Entity<Follower>(entity =>
        {
            entity.HasKey(e => e.FollowId).HasName("PK__Follower__2CE8108E7D02D8A2");

            entity.HasIndex(e => e.FollowerUserId, "IX_Followers_FollowerUserID");

            entity.HasIndex(e => e.FollowingUserId, "IX_Followers_FollowingUserID");

            entity.HasIndex(e => new { e.FollowerUserId, e.FollowingUserId }, "UQ_Followers_FollowerFollowing").IsUnique();

            entity.Property(e => e.FollowId).HasColumnName("FollowID");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.FollowerUserId).HasColumnName("FollowerUserID");
            entity.Property(e => e.FollowingUserId).HasColumnName("FollowingUserID");

            entity.HasOne(d => d.FollowerUser).WithMany(p => p.FollowerFollowerUsers)
                .HasForeignKey(d => d.FollowerUserId)
                .HasConstraintName("FK_Followers_FollowerUser");

            entity.HasOne(d => d.FollowingUser).WithMany(p => p.FollowerFollowingUsers)
                .HasForeignKey(d => d.FollowingUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Followers_FollowingUser");
        });

        modelBuilder.Entity<MealPlan>(entity =>
        {
            entity.HasKey(e => e.MealPlanId).HasName("PK__MealPlan__0620DB56FE05D77E");

            entity.HasIndex(e => e.PlanDate, "IX_MealPlans_PlanDate");

            entity.HasIndex(e => new { e.UserId, e.PlanDate }, "IX_MealPlans_UserDate");

            entity.HasIndex(e => e.UserId, "IX_MealPlans_UserID");

            entity.Property(e => e.MealPlanId).HasColumnName("MealPlanID");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.MealType).HasMaxLength(20);
            entity.Property(e => e.Notes).HasMaxLength(500);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.MealPlans)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_MealPlans_Users");
        });

        modelBuilder.Entity<MealPlanRecipe>(entity =>
        {
            entity.HasKey(e => e.MealPlanRecipeId).HasName("PK__MealPlan__DB8DF4244529C1CE");

            entity.HasIndex(e => e.MealPlanId, "IX_MealPlanRecipes_MealPlanID");

            entity.HasIndex(e => e.RecipeId, "IX_MealPlanRecipes_RecipeID");

            entity.Property(e => e.MealPlanRecipeId).HasColumnName("MealPlanRecipeID");
            entity.Property(e => e.MealPlanId).HasColumnName("MealPlanID");
            entity.Property(e => e.RecipeId).HasColumnName("RecipeID");
            entity.Property(e => e.Servings).HasDefaultValue(4);

            entity.HasOne(d => d.MealPlan).WithMany(p => p.MealPlanRecipes)
                .HasForeignKey(d => d.MealPlanId)
                .HasConstraintName("FK_MealPlanRecipes_MealPlans");

            entity.HasOne(d => d.Recipe).WithMany(p => p.MealPlanRecipes)
                .HasForeignKey(d => d.RecipeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MealPlanRecipes_Recipes");
        });

        modelBuilder.Entity<Recipe>(entity =>
        {
            entity.HasKey(e => e.RecipeId).HasName("PK__Recipes__FDD988D03D04DBD1");

            entity.HasIndex(e => e.CategoryId, "IX_Recipes_CategoryID");

            entity.HasIndex(e => e.CreatedAt, "IX_Recipes_CreatedAt").IsDescending();

            entity.HasIndex(e => e.IsPublic, "IX_Recipes_IsPublic");

            entity.HasIndex(e => e.UserId, "IX_Recipes_UserID");

            entity.Property(e => e.RecipeId).HasColumnName("RecipeID");
            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.Difficulty).HasMaxLength(20);
            entity.Property(e => e.IsPublic).HasDefaultValue(true);
            entity.Property(e => e.Servings).HasDefaultValue(4);
            entity.Property(e => e.Title).HasMaxLength(200);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.ViewCount).HasDefaultValue(0);

            entity.HasOne(d => d.Category).WithMany(p => p.Recipes)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Recipes_Categories");

            entity.HasOne(d => d.User).WithMany(p => p.Recipes)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Recipes_Users");
        });

        modelBuilder.Entity<RecipeImage>(entity =>
        {
            entity.HasKey(e => e.ImageId).HasName("PK__RecipeIm__7516F4EC5A3830E5");

            entity.HasIndex(e => new { e.RecipeId, e.ImageOrder }, "IX_RecipeImages_ImageOrder");

            entity.HasIndex(e => e.RecipeId, "IX_RecipeImages_RecipeID");

            entity.Property(e => e.ImageId).HasColumnName("ImageID");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.ImageOrder).HasDefaultValue(0);
            entity.Property(e => e.ImageUrl).HasMaxLength(500);
            entity.Property(e => e.RecipeId).HasColumnName("RecipeID");

            entity.HasOne(d => d.Recipe).WithMany(p => p.RecipeImages)
                .HasForeignKey(d => d.RecipeId)
                .HasConstraintName("FK_RecipeImages_Recipes");
        });

        modelBuilder.Entity<RecipeIngredient>(entity =>
        {
            entity.HasKey(e => e.IngredientId).HasName("PK__RecipeIn__BEAEB27A938C2112");

            entity.HasIndex(e => e.RecipeId, "IX_RecipeIngredients_RecipeID");

            entity.Property(e => e.IngredientId).HasColumnName("IngredientID");
            entity.Property(e => e.IngredientName).HasMaxLength(100);
            entity.Property(e => e.OrderIndex).HasDefaultValue(0);
            entity.Property(e => e.Quantity).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.RecipeId).HasColumnName("RecipeID");
            entity.Property(e => e.Unit).HasMaxLength(50);

            entity.HasOne(d => d.Recipe).WithMany(p => p.RecipeIngredients)
                .HasForeignKey(d => d.RecipeId)
                .HasConstraintName("FK_RecipeIngredients_Recipes");
        });

        modelBuilder.Entity<RecipeLike>(entity =>
        {
            entity.HasKey(e => e.LikeId).HasName("PK__RecipeLi__A2922CF4BA113ED1");

            entity.HasIndex(e => e.RecipeId, "IX_RecipeLikes_RecipeID");

            entity.HasIndex(e => e.UserId, "IX_RecipeLikes_UserID");

            entity.HasIndex(e => new { e.RecipeId, e.UserId }, "UQ_RecipeLikes_RecipeUser").IsUnique();

            entity.Property(e => e.LikeId).HasColumnName("LikeID");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.RecipeId).HasColumnName("RecipeID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Recipe).WithMany(p => p.RecipeLikes)
                .HasForeignKey(d => d.RecipeId)
                .HasConstraintName("FK_RecipeLikes_Recipes");

            entity.HasOne(d => d.User).WithMany(p => p.RecipeLikes)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RecipeLikes_Users");
        });

        modelBuilder.Entity<RecipeStep>(entity =>
        {
            entity.HasKey(e => e.StepId).HasName("PK__RecipeSt__243433377DE8AD4B");

            entity.HasIndex(e => e.RecipeId, "IX_RecipeSteps_RecipeID");

            entity.HasIndex(e => new { e.RecipeId, e.StepNumber }, "IX_RecipeSteps_StepNumber");

            entity.Property(e => e.StepId).HasColumnName("StepID");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.ImageUrl).HasMaxLength(500);
            entity.Property(e => e.RecipeId).HasColumnName("RecipeID");
            entity.Property(e => e.StepDescription).HasMaxLength(1000);

            entity.HasOne(d => d.Recipe).WithMany(p => p.RecipeSteps)
                .HasForeignKey(d => d.RecipeId)
                .HasConstraintName("FK_RecipeSteps_Recipes");
        });

        modelBuilder.Entity<SavedRecipe>(entity =>
        {
            entity.HasKey(e => e.SaveId).HasName("PK__SavedRec__1450D386715E9DFC");

            entity.HasIndex(e => e.RecipeId, "IX_SavedRecipes_RecipeID");

            entity.HasIndex(e => e.UserId, "IX_SavedRecipes_UserID");

            entity.HasIndex(e => new { e.RecipeId, e.UserId }, "UQ_SavedRecipes_RecipeUser").IsUnique();

            entity.Property(e => e.SaveId).HasColumnName("SaveID");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.RecipeId).HasColumnName("RecipeID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Recipe).WithMany(p => p.SavedRecipes)
                .HasForeignKey(d => d.RecipeId)
                .HasConstraintName("FK_SavedRecipes_Recipes");

            entity.HasOne(d => d.User).WithMany(p => p.SavedRecipes)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SavedRecipes_Users");
        });

        modelBuilder.Entity<ShoppingList>(entity =>
        {
            entity.HasKey(e => e.ShoppingListId).HasName("PK__Shopping__6CBBDD748C3C1401");

            entity.HasIndex(e => e.UserId, "IX_ShoppingLists_UserID");

            entity.Property(e => e.ShoppingListId).HasColumnName("ShoppingListID");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.ListName)
                .HasMaxLength(100)
                .HasDefaultValue("My Shopping List");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.ShoppingLists)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_ShoppingLists_Users");
        });

        modelBuilder.Entity<ShoppingListItem>(entity =>
        {
            entity.HasKey(e => e.ItemId).HasName("PK__Shopping__727E83EBF85EA64F");

            entity.HasIndex(e => e.Category, "IX_ShoppingListItems_Category");

            entity.HasIndex(e => e.RecipeId, "IX_ShoppingListItems_RecipeID");

            entity.HasIndex(e => e.ShoppingListId, "IX_ShoppingListItems_ShoppingListID");

            entity.Property(e => e.ItemId).HasColumnName("ItemID");
            entity.Property(e => e.Category).HasMaxLength(50);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IngredientName).HasMaxLength(100);
            entity.Property(e => e.IsChecked).HasDefaultValue(false);
            entity.Property(e => e.Quantity).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.RecipeId).HasColumnName("RecipeID");
            entity.Property(e => e.ShoppingListId).HasColumnName("ShoppingListID");
            entity.Property(e => e.Unit).HasMaxLength(50);

            entity.HasOne(d => d.Recipe).WithMany(p => p.ShoppingListItems)
                .HasForeignKey(d => d.RecipeId)
                .HasConstraintName("FK_ShoppingListItems_Recipes");

            entity.HasOne(d => d.ShoppingList).WithMany(p => p.ShoppingListItems)
                .HasForeignKey(d => d.ShoppingListId)
                .HasConstraintName("FK_ShoppingListItems_ShoppingLists");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CCAC971B7A58");

            entity.HasIndex(e => e.Email, "IX_Users_Email");

            entity.HasIndex(e => e.Username, "IX_Users_Username");

            entity.HasIndex(e => e.Username, "UQ__Users__536C85E49BC028CD").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__Users__A9D10534B0286A86").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.Bio).HasMaxLength(500);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Password).HasMaxLength(255);
            entity.Property(e => e.ProfileImage).HasMaxLength(255);
            entity.Property(e => e.Role)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasDefaultValue("User");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Username).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
