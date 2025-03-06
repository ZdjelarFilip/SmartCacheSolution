using TraditionalAPI.Dto;
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

        public async Task<bool> MarkAsBreachedAsync(string email)
        {
            if (await _breachedEmailStorage.ExistsAsync(email))
            {
                return false; // Email already exists
            }

            var newBreach = new BreachedEmail
            {
                Email = email,
                BreachDate = DateTime.UtcNow
            };

            await _breachedEmailStorage.AddAsync(newBreach);

            return true;
        }
    }
}