using SmartCacheAPI.Grains;
using SmartCacheAPI.Models;

namespace SmartCacheApi.Grains
{
    public class BreachedEmailGrain : Grain, IBreachedEmailGrain, IRemindable
    {
        private readonly IPersistentState<BreachedEmail> _state;
        private readonly ILogger<BreachedEmailGrain> _logger;
        private IDisposable? _timer;

        public BreachedEmailGrain(
            [PersistentState("breach", "nomnioresource")] IPersistentState<BreachedEmail> state,
            ILogger<BreachedEmailGrain> logger)
        {
            _state = state;
            _logger = logger;
        }

        public override async Task OnActivateAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("BreachedEmailGrain activated for key: {Key}", this.GetPrimaryKeyString());

            // Ensure state is loaded from storage on activation
            await _state.ReadStateAsync();

            try
            {
                await this.RegisterOrUpdateReminder("save-to-blob-storage", TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(5));

                _timer = this.RegisterGrainTimer<object?>(
                    PersistStateAsync,
                    state: null,
                    TimeSpan.FromMinutes(5), // DueTime
                    TimeSpan.FromMinutes(5)  // Period
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to register reminder for {Key}", this.GetPrimaryKeyString());
            }

            await base.OnActivateAsync(cancellationToken);
        }

        public async Task ReceiveReminder(string reminderName, TickStatus status)
        {
            _logger.LogInformation("Received reminder: {ReminderName} for grain {Key}", reminderName, this.GetPrimaryKeyString());

            if (reminderName == "save-to-blob-storage")
            {
                await PersistStateAsync(null);
            }
        }

        private async Task PersistStateAsync(object? state)
        {
            try
            {
                if (_state.State != null)
                {
                    _logger.LogInformation("Persisting state for {Key}", this.GetPrimaryKeyString());
                    await _state.WriteStateAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error persisting state for {Key}", this.GetPrimaryKeyString());
            }
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