namespace SmartCacheAPI.Models
{
    [GenerateSerializer]
    [Alias("SmartCacheApi.Models.BreachedEmail")]
    public class BreachedEmail
    {
        [Id(0)]
        public required string Email { get; set; }

        [Id(1)]
        public DateTime? BreachDate { get; set; }
    }
}