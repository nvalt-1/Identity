using Identity.Models.Identity;

namespace Identity.Services.Interfaces;

/// <summary>
/// Used to bridge UserStore for Identity and the database.
/// Methods should never propagate exceptions to the consumer.
/// </summary>
public interface IIdentityService : IDisposable
{
    /// <summary>
    /// Searches SQL USER table and returns a constructed User object, or null if
    /// not found.
    /// </summary>
    /// <param name="userId">User id</param>
    /// <returns>Returns null if not found</returns>
    public Task<ApplicationUser?> FindUserById(string userId);

    /// <summary>
    /// Searches SQL USER table and returns a constructed User object, or null if
    /// not found.
    /// </summary>
    /// <param name="normalizedUserName">Normalized username</param>
    /// <returns>Returns null if not found</returns>
    public Task<ApplicationUser?> FindActiveUserByUserName(string normalizedUserName);

    /// <summary>
    /// Marks the provided user as archived (soft delete)
    /// </summary>
    /// <param name="userId">User id</param>
    /// <returns>True if successful</returns>
    public Task<bool> ArchiveUser(string userId);

    /// <summary>
    /// Inserts the user into the SQL database
    /// </summary>
    /// <param name="applicationUser">User to create</param>
    /// <returns>True if successful</returns>
    public Task<bool> CreateUser(ApplicationUser applicationUser);

    /// <summary>
    /// Updates the user in the SQL database
    /// </summary>
    /// <param name="applicationUser">User to update</param>
    /// <returns>True if successful</returns>
    public Task<bool> UpdateUser(ApplicationUser applicationUser);
}
