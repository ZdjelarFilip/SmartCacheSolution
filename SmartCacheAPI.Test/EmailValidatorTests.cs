using SmartCacheAPI.Helpers;

namespace SmartCacheAPI.Tests
{
    public class EmailValidatorTests
    {
        [Theory]
        [InlineData("valid@example.com", true)]
        [InlineData("invalid-email", false)]
        [InlineData("another.valid+test@example.com", true)]
        public void IsValidEmail_ValidatesCorrectly(string email, bool expected)
        {
            var result = EmailValidator.IsValidEmail(email);
            Assert.Equal(expected, result);
        }
    }
}