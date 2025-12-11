using Microsoft.EntityFrameworkCore;
using RecipeSharingPlatform.Repository.Basic;
using RecipeSharingPlatform.Repository.DbContexts;
using RecipeSharingPlatform.Repository.Models;
using RecipeSharingPlatform.Repository.Repositories.Interfaces;

namespace RecipeSharingPlatform.Repository.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(RecipeSharingDbContext _context) : base(_context)
        {

        }

        public async Task<User> Login(string username, string password)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => 
                u.Username == username && 
                u.Password == password && 
                u.IsActive == true
                );
        }
    }
}
