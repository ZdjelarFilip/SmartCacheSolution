namespace TraditionalAPI.Models
{
    public class BreachedEmail
    {
        public Guid Id { get; set; } = new Guid();
        public required string Email { get; set; }
        public DateTime BreachDate { get; set; }
    }
}