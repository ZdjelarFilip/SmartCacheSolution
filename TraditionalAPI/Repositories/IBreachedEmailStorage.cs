using TraditionalAPI.Models;

namespace TraditionalAPI.Repositories
{
    public interface IBreachedEmailStorage
    {
        /// <summary>
        /// Checks if the given email exists in the breached list.
        /// </summary>
        Task<bool> ExistsAsync(string email);

        /// <summary>
        /// Adds a new breached email to the storage.
        /// </summary>
        Task AddAsync(BreachedEmail breach);
    }
}