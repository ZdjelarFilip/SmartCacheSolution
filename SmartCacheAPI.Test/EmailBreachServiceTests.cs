using Moq;
using Microsoft.Extensions.Logging;
using SmartCacheAPI.Grains;
using SmartCacheAPI.Services;

namespace SmartCacheAPI.Tests
{
    public class EmailBreachServiceTests
    {
        private readonly Mock<IClusterClient> _orleansClientMock;
        private readonly Mock<ILogger<EmailBreachService>> _loggerMock;
        private readonly EmailBreachService _service;

        public EmailBreachServiceTests()
        {
            _orleansClientMock = new Mock<IClusterClient>();
            _loggerMock = new Mock<ILogger<EmailBreachService>>();
            _service = new EmailBreachService(_orleansClientMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task IsEmailBreachedAsync_ReturnsTrue_WhenBreached()
        {
            var grainMock = new Mock<IBreachedEmailGrain>();
            grainMock.Setup(g => g.IsBreachedAsync()).ReturnsAsync(true);
            _orleansClientMock.Setup(c => c.GetGrain<IBreachedEmailGrain>(It.IsAny<string>(), null))
                .Returns(grainMock.Object);

            var result = await _service.IsEmailBreachedAsync("test@example.com");
            Assert.True(result);
        }

        [Fact]
        public async Task IsEmailBreachedAsync_ReturnsFalse_WhenNotBreached()
        {
            var grainMock = new Mock<IBreachedEmailGrain>();
            grainMock.Setup(g => g.IsBreachedAsync()).ReturnsAsync(false);
            _orleansClientMock.Setup(c => c.GetGrain<IBreachedEmailGrain>(It.IsAny<string>(), null))
                .Returns(grainMock.Object);

            var result = await _service.IsEmailBreachedAsync("test@example.com");
            Assert.False(result);
        }
    }
}