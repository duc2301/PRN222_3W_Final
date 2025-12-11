using RecipeSharingPlatform.Repository.Basic.Interfaces;
using Microsoft.EntityFrameworkCore;
using RecipeSharingPlatform.Repository.DbContexts;

namespace RecipeSharingPlatform.Repository.Basic
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        public readonly RecipeSharingDbContext _context;

        public GenericRepository(RecipeSharingDbContext context)
        {
            _context = context;
        }

        public async Task<T> CreateAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
            return entity;
        }

        public async Task<List<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public void Remove(T entity)
        {
            _context.Set<T>().Remove(entity);
        }

        public void Update(T entity)
        {
            _context.ChangeTracker.Clear();
            var tracker = _context.Attach(entity);
            tracker.State = EntityState.Modified;
        }
    }
}
