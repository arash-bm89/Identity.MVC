using Identity.MVC.Models;

namespace Identity.MVC.Repository.IRepository
{
    public interface IRepository<T> where T: class
    {
        Task<T?> GetByIdAsync(int id);
        T? GetById(int id);
        Task<T> CreateAsync(T entity);
        Task<T> UpdateAsync(T entity);
        Task DeleteAsync(T entity);
        Task SaveChangesAsync();
    }
}
