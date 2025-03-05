using Microsoft.EntityFrameworkCore;
using TraditionalAPI.Models;

namespace TraditionalAPI.Data
{
    public class BreachDbContext : DbContext
    {
        public BreachDbContext(DbContextOptions<BreachDbContext> options) : base(options) { }

        public DbSet<BreachedEmail> BreachedEmails { get; set; }
    }
}