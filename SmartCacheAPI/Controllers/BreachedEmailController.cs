using Microsoft.AspNetCore.Mvc;
using SmartCacheApi.Services;
using SmartCacheAPI.Models;

namespace SmartCacheAPI.Controllers
{
    [ApiController]
    [Route("api/breaches")]
    public class BreachedEmailsController : ControllerBase
    {
        private readonly EmailBreachService _emailBreachService;
        private readonly ILogger<BreachedEmailsController> _logger;

        public BreachedEmailsController(EmailBreachService emailBreachService, ILogger<BreachedEmailsController> logger)
        {
            _emailBreachService = emailBreachService;
            _logger = logger;
        }

        [HttpGet("{email}")]
        public async Task<IActionResult> IsEmailBreachedAsync(string email)
        {

            bool isBreached = await _emailBreachService.IsEmailBreachedAsync(email);
            return isBreached ? Ok("Ok") : NotFound("Not Found");
        }

        [HttpPost]
        public async Task<IActionResult> MarkAsBreachedAsync([FromBody] BreachedEmail breach)
        {
            bool success = await _emailBreachService.MarkAsBreachedAsync(breach);
            return success ? Created() : Conflict("Email already exists in breach list");           
        }
    }
}