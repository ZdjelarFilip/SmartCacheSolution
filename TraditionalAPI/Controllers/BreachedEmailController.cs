using Microsoft.AspNetCore.Mvc;
using TraditionalAPI.Helpers;
using TraditionalAPI.Models;
using TraditionalAPI.Services;

namespace TraditionalAPI.Controllers
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
            try
            {
                if (!EmailValidator.IsValidEmail(email))
                    return BadRequest("Invalid email format.");

                bool isBreached = await _emailBreachService.IsEmailBreachedAsync(email);
                return isBreached ? Ok("Ok") : NotFound("Not Found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while checking email breach for {Email}", email);
                return StatusCode(500, "An unexpected error occurred.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> MarkAsBreachedAsync([FromBody] BreachedEmail breach)
        {
            try
            {
                if (!EmailValidator.IsValidEmail(breach.Email))
                    return BadRequest("Invalid email format.");

                bool success = await _emailBreachService.MarkAsBreachedAsync(breach);
                return success ? Created() : Conflict("Email already exists in breach list");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while marking email as breached: {Email}", breach.Email);
                return StatusCode(500, "An unexpected error occurred.");
            }
        }
    }
}
