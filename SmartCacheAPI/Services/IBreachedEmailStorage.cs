using SmartCacheAPI.Models;

namespace SmartCacheAPI.Services
{
    public interface IBreachedEmailStorage
    {
        Task<bool> ExistsAsync(string email);
        Task AddAsync(BreachedEmail breach);
    }
}
