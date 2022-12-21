using Identity.MVC.Data;
using Identity.MVC.Models;
using Identity.MVC.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace Identity.MVC.Repository
{
    public class UserRepository: Repository<User>, IUserRepository
    {
        public UserRepository(ApplicationDbContext db) : base(db)
        {
        }

        public async Task<bool> IsUsernameUnique(string username)
        {
            var user = await _dbset
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Username == username);
            return user == null;
        }

        public async Task<User?> GetUserFromUsernamePasswordAsync(string username, string password)
        {
            var user = await _dbset.AsNoTracking()
                .FirstOrDefaultAsync(u => u.Username == username && u.Password == password);
            return user;
        }
    }
}
