using Microsoft.EntityFrameworkCore;
using TraditionalAPI.Data;
using TraditionalAPI.Models;

namespace TraditionalAPI.Repositories
{
    public class BreachedEmailStorage : IBreachedEmailStorage
    {
        private readonly BreachDbContext _context;

        public BreachedEmailStorage(BreachDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ExistsAsync(string email)
        {
            return await _context.BreachedEmails.AnyAsync(e => e.Email == email);
        }

        public async Task AddAsync(BreachedEmail breach)
        {
            _context.BreachedEmails.Add(breach);
            await _context.SaveChangesAsync();
        }
    }
}