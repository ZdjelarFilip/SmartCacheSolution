using TraditionalAPI.Models;
using TraditionalAPI.Repositories;

namespace TraditionalAPI.Services
{
    public class EmailBreachService
    {
        private readonly IBreachedEmailStorage _breachedEmailStorage;

        public EmailBreachService(IBreachedEmailStorage breachedEmailStorage)
        {
            _breachedEmailStorage = breachedEmailStorage;
        }

        public async Task<bool> IsEmailBreachedAsync(string email)
        {
            return await _breachedEmailStorage.ExistsAsync(email);
        }

        public async Task<bool> MarkAsBreachedAsync(BreachedEmail breach)
        {
            if (await _breachedEmailStorage.ExistsAsync(breach.Email))
            {
                return false; // Email already exists
            }

            breach.BreachDate = DateTime.UtcNow;
            await _breachedEmailStorage.AddAsync(breach);

            return true;
        }
    }
}