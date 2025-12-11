using RecipeSharingPlatform.Repository.DbContexts;
using RecipeSharingPlatform.Repository.Repositories;
using RecipeSharingPlatform.Repository.Repositories.Interfaces;
using RecipeSharingPlatform.Repository.UnitOfWork.Interface;

namespace RecipeSharingPlatform.Repository.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly RecipeSharingDbContext _context;

        public UnitOfWork(RecipeSharingDbContext context)
        {
            _context = context;
        }

        private IUserRepository _userRepository;
        public IUserRepository UserRepository => _userRepository ??  new UserRepository(_context);

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
