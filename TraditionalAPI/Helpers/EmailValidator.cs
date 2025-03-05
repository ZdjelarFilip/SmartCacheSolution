using System.Text.RegularExpressions;

namespace TraditionalAPI.Helpers
{
    public class EmailValidator
    {
        public static bool IsValidEmail(string email)
        {
            return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }
    }
}