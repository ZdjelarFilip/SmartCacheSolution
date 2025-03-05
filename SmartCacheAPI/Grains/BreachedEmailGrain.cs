using SmartCacheAPI.Models;

namespace SmartCacheAPI.Grains
{
    public class BreachedEmailGrain : Grain, IBreachedEmailGrain
    {
        private readonly IPersistentState<BreachedEmail> _state;
        private readonly ILogger<BreachedEmailGrain> _logger;

        public BreachedEmailGrain(
            [PersistentState("breach", "nomnioresource")] IPersistentState<BreachedEmail> state,
            ILogger<BreachedEmailGrain> logger)
        {
            _state = state;
            _logger = logger;
        }

        public async Task<bool> IsBreachedAsync()
        {
            _logger.LogDebug("Checking if email is breached for {Key}", this.GetPrimaryKeyString());
            return await Task.FromResult(!string.IsNullOrWhiteSpace(_state.State?.Email));
        }

        public async Task MarkAsBreachedAsync(BreachedEmail breach)
        {
            if (_state.State.Email == null)
            {
                _logger.LogInformation("Marking email {Email} as breached in grain {Key}", breach.Email, this.GetPrimaryKeyString());

                _state.State = breach;
                await _state.WriteStateAsync();
            }
            else
            {
                _logger.LogWarning("Attempted to mark already breached email {Email} in grain {Key}", breach.Email, this.GetPrimaryKeyString());
            }
        }
    }
}