using SmartCacheAPI.Grains;
using SmartCacheAPI.Models;

namespace SmartCacheAPI.Services
{
    public class EmailBreachService
    {
        private readonly IClusterClient _orleansClient;
        private readonly ILogger<EmailBreachService> _logger;

        public EmailBreachService(IClusterClient orleansClient, ILogger<EmailBreachService> logger)
        {
            _orleansClient = orleansClient;
            _logger = logger;
        }

        public async Task<bool> IsEmailBreachedAsync(string email)
        {
            try
            {
                var grain = _orleansClient.GetGrain<IBreachedEmailGrain>(email);
                return await grain.IsBreachedAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if email {Email} is breached", email);
                return false; 
            }
        }

        public async Task<bool> MarkAsBreachedAsync(string email)
        {
            try
            {
                var grain = _orleansClient.GetGrain<IBreachedEmailGrain>(email);
                if (await grain.IsBreachedAsync())
                {
                    _logger.LogInformation("Email {Email} is already marked as breached", email);
                    return false;
                }

                var breach = new BreachedEmail { Email = email };
                breach.BreachDate = DateTime.UtcNow;
                
                await grain.MarkAsBreachedAsync(breach);
                _logger.LogInformation("Email {Email} successfully marked as breached", breach.Email);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking email {Email} as breached", email);
                return false;
            }
        }
    }
}