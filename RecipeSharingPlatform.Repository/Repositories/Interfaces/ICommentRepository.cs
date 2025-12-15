using RecipeSharingPlatform.Repository.Models;

public interface ICommentRepository
{
    Task<List<Comment>> GetCommentsByRecipeIdAsync(int recipeId);
    Task AddAsync(Comment comment);
    Task SaveChangesAsync();
}