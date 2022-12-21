using Identity.MVC.Models;

namespace Identity.MVC.Repository.IRepository
{
    public interface IUserRepository: IRepository<User>
    {
        Task<bool> IsUsernameUnique(string username);
        Task<User?> GetUserFromUsernamePasswordAsync(string username, string password);
    }
}
