using Microsoft.EntityFrameworkCore;
using RecipeSharingPlatform.Repository.DbContexts;
using RecipeSharingPlatform.Repository.Models;

public class CommentRepository : ICommentRepository
{
    private readonly RecipeSharingDbContext _context;
    public CommentRepository(RecipeSharingDbContext context)
    {
        _context = context;
    }

    public async Task<List<Comment>> GetCommentsByRecipeIdAsync(int recipeId)
    {
        return await _context.Comments
            .Where(c => c.RecipeId == recipeId && c.ParentCommentId == null)
            .Include(c => c.User)
            .Include(c => c.InverseParentComment)
                .ThenInclude(r => r.User)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task AddAsync(Comment comment)
    {
        await _context.Comments.AddAsync(comment);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}