using RecipeSharingPlatform.Repository.DbContexts;
using RecipeSharingPlatform.Repository.Repositories;
using RecipeSharingPlatform.Repository.Repositories.Interfaces;
using RecipeSharingPlatform.Repository.UnitOfWork.Interface;

namespace RecipeSharingPlatform.Repository.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly RecipeSharingDbContext _context;

        public IRecipeRepository RecipeRepository { get; }
        public ICategoryRepository CategoryRepository { get; }

        public UnitOfWork(RecipeSharingDbContext context, IRecipeRepository recipeRepository,
        ICategoryRepository categoryRepository)
        {
            _context = context;
            RecipeRepository = recipeRepository;
            CategoryRepository = categoryRepository;
        }

        private IUserRepository _userRepository;
        public IUserRepository UserRepository => _userRepository ??  new UserRepository(_context);

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
