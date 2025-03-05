using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SmartCacheAPI.Controllers;
using SmartCacheAPI.Grains;
using SmartCacheAPI.Models;
using SmartCacheAPI.Services;

namespace SmartCacheAPI.Tests
{
    public class BreachedEmailsControllerTests
    {
        private readonly Mock<IClusterClient> _clusterClientMock;
        private readonly Mock<IBreachedEmailGrain> _breachedEmailGrainMock;
        private readonly Mock<ILogger<BreachedEmailsController>> _loggerMock;
        private readonly EmailBreachService _service;
        private readonly BreachedEmailsController _controller;

        public BreachedEmailsControllerTests()
        {
            _clusterClientMock = new Mock<IClusterClient>();
            _breachedEmailGrainMock = new Mock<IBreachedEmailGrain>();

            // Set up the Orleans grain mock
            _clusterClientMock.Setup(c => c.GetGrain<IBreachedEmailGrain>(It.IsAny<string>(), null))
                .Returns(_breachedEmailGrainMock.Object);

            _service = new EmailBreachService(_clusterClientMock.Object, Mock.Of<ILogger<EmailBreachService>>());
            _loggerMock = new Mock<ILogger<BreachedEmailsController>>();
            _controller = new BreachedEmailsController(_service, _loggerMock.Object);
        }

        [Fact]
        public async Task IsEmailBreachedAsync_ReturnsOk_WhenEmailIsBreached()
        {
            _breachedEmailGrainMock.Setup(g => g.IsBreachedAsync()).ReturnsAsync(true);

            var result = await _controller.IsEmailBreachedAsync("breached@example.com");
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Ok", okResult.Value);
        }

        [Fact]
        public async Task IsEmailBreachedAsync_ReturnsNotFound_WhenEmailIsNotBreached()
        {
            _breachedEmailGrainMock.Setup(g => g.IsBreachedAsync()).ReturnsAsync(false);

            var result = await _controller.IsEmailBreachedAsync("safe@example.com");
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task MarkAsBreachedAsync_ReturnsCreated_WhenSuccessful()
        {
            _breachedEmailGrainMock.Setup(g => g.IsBreachedAsync()).ReturnsAsync(false);
            _breachedEmailGrainMock.Setup(g => g.MarkAsBreachedAsync(It.IsAny<BreachedEmail>())).Returns(Task.CompletedTask);

            var breach = new BreachedEmail { Email = "test@example.com" };
            var result = await _controller.MarkAsBreachedAsync(breach);
            Assert.IsType<CreatedResult>(result);
        }

        [Fact]
        public async Task MarkAsBreachedAsync_ReturnsConflict_WhenEmailAlreadyExists()
        {
            _breachedEmailGrainMock.Setup(g => g.IsBreachedAsync()).ReturnsAsync(true);

            var breach = new BreachedEmail { Email = "test@example.com" };
            var result = await _controller.MarkAsBreachedAsync(breach);
            Assert.IsType<ConflictObjectResult>(result);
        }
    }
}