
namespace RecipeSharingPlatform.Repository.Basic.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task<List<T>> GetAllAsync();
        Task<T> GetByIdAsync(int id);
        Task<T> CreateAsync(T entity);
        void Update(T entity);
        void Remove(T entity);
    }
}
