namespace Identity.MVC.Repository.IRepository
{
    public interface ICacheRepository<TValue>
    where TValue : class
    {
        Task<TValue> SetAsync(string key, TValue value);
        Task<TValue?> GetAsync(string key);
        TValue? Get(string key);
        Task<TValue> UpdateAsync(string key, TValue value);
        Task DeleteAsync(string key);
    }
}
