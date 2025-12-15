using RecipeSharingPlatform.Repository.Models;
namespace RecipeSharingPlatform.Repository.Repositories.Interfaces;
public interface ICommentRepository
{
    Task<List<Comment>> GetCommentsByRecipeIdAsync(int recipeId);
    Task AddAsync(Comment comment);
    Task SaveChangesAsync();
    Task<Comment?> GetByIdAsync(int id);
    void Delete(Comment comment);
}