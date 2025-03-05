using System.Text.RegularExpressions;

namespace SmartCacheAPI.Helpers
{
    public class EmailValidator
    {
        public static bool IsValidEmail(string email)
        {
            return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }
    }
}
