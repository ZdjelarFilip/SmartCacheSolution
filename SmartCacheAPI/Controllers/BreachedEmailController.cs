using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartCacheAPI.Services;
using SmartCacheAPI.Models;
using SmartCacheAPI.Helpers;

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

        [Authorize]
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
            catch (OrleansException ex)
            {
                _logger.LogWarning(ex, "Orleans error while checking email breach for {Email}", email);
                return StatusCode(503, "Service unavailable, please try again later.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while checking email breach for {Email}", email);
                return StatusCode(500, "An unexpected error occurred.");
            }
        }

        [Authorize]
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
            catch (OrleansException ex)
            {
                _logger.LogWarning(ex, "Orleans error while marking email as breached: {Email}", breach.Email);
                return StatusCode(503, "Service unavailable, please try again later.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while marking email as breached: {Email}", breach.Email);
                return StatusCode(500, "An unexpected error occurred.");
            }
        }
    }
}