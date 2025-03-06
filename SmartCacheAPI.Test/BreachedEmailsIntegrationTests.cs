using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Net;
using System.Net.Http.Headers;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Mvc.Testing;

namespace SmartCacheAPI.Tests
{
    public class BreachedEmailsIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly string _jwtToken;

        public BreachedEmailsIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
            _jwtToken = GenerateJwtToken();
        }

        private string GenerateJwtToken()
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("8fd3938f-b5b1-4733-b62e-ed38318d2ed6"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, "testuser") }),
                Expires = DateTime.UtcNow.AddHours(1),
                Issuer = "FZ",
                Audience = "general-public",
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        [Fact]
        public async Task GetEmailStatus_ReturnsNotFound_WhenEmailNotBreached()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/breaches/safe@example.com");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _jwtToken);

            var response = await _client.SendAsync(request);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task PostEmail_AddsEmailToBreachList()
        {
            var email = $"breached.{Guid.NewGuid()}@example.com";

            var request = new HttpRequestMessage(HttpMethod.Post, $"/api/breaches?email={email}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _jwtToken);

            var response = await _client.SendAsync(request);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }
    }
}