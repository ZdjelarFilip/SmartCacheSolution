using SmartCacheAPI.Models;

namespace SmartCacheAPI.Grains
{
    public interface IBreachedEmailGrain : IGrainWithStringKey
    {
        Task<bool> IsBreachedAsync();
        Task MarkAsBreachedAsync(BreachedEmail breach);
    }
}