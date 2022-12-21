using Identity.MVC.Data;
using Identity.MVC.Models;
using Identity.MVC.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace Identity.MVC.Repository
{
    public class Repository<T>: IRepository<T> where T: ModelBase
    {
        private readonly ApplicationDbContext _db;
        internal DbSet<T> _dbset;


        public Repository(ApplicationDbContext db)
        {
            _db = db;
            _dbset = _db.Set<T>();
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            var entity = await _dbset
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == id);
            return entity;
        }

        public T? GetById(int id)
        {
            var entity = _dbset
                .AsNoTracking()
                .FirstOrDefault(e => e.Id == id);
            return entity;
        }

        public async Task<T> CreateAsync(T entity)
        {
            await _dbset.AddAsync(entity);
            await SaveChangesAsync();
            return entity;
        }

        public async Task<T> UpdateAsync(T entity)
        {
            _dbset.Update(entity);
            await SaveChangesAsync();
            return entity;
        }

        public async Task DeleteAsync(T entity)
        {
            _dbset.Remove(entity);
            await SaveChangesAsync();
            
        }

        public async Task SaveChangesAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}
