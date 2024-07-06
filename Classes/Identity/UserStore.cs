using System.Diagnostics;
using Identity.Models.Identity;
using Identity.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Identity.Classes.Identity;

public class UserStore :
    IUserPasswordStore<ApplicationUser>,
    IUserSecurityStampStore<ApplicationUser>
{
    private readonly IIdentityService _identityService;

    public UserStore(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public Task<string> GetUserIdAsync(ApplicationUser applicationUser, CancellationToken cancellationToken)
    {
        return Task.FromResult(applicationUser.Id);
    }

    public Task<string?> GetUserNameAsync(ApplicationUser applicationUser, CancellationToken cancellationToken)
    {
        return Task.FromResult(applicationUser.UserName);
    }

    public Task SetUserNameAsync(ApplicationUser applicationUser, string? userName, CancellationToken cancellationToken)
    {
        applicationUser.UserName = userName;

        return Task.CompletedTask;
    }

    public Task<string?> GetNormalizedUserNameAsync(ApplicationUser applicationUser, CancellationToken cancellationToken)
    {
        return Task.FromResult(applicationUser.NormalizedUserName);
    }

    public Task SetNormalizedUserNameAsync(ApplicationUser applicationUser, string? normalizedName, CancellationToken cancellationToken)
    {
        applicationUser.NormalizedUserName = normalizedName;
        return Task.CompletedTask;
    }

    public async Task<IdentityResult> CreateAsync(ApplicationUser applicationUser, CancellationToken cancellationToken)
    {
        Debug.Assert(string.IsNullOrEmpty(applicationUser.Id));
        return await _identityService.CreateUser(applicationUser) ? IdentityResult.Success : IdentityResult.Failed();
    }

    public async Task<IdentityResult> UpdateAsync(ApplicationUser applicationUser, CancellationToken cancellationToken)
    {
        Debug.Assert(applicationUser.Id != null);
        return await _identityService.UpdateUser(applicationUser) ? IdentityResult.Success : IdentityResult.Failed();
    }

    public async Task<IdentityResult> DeleteAsync(ApplicationUser applicationUser, CancellationToken cancellationToken)
    {
        return await _identityService.ArchiveUser(applicationUser.Id) ? IdentityResult.Success : IdentityResult.Failed();
    }

    public Task<ApplicationUser?> FindByIdAsync(string userId, CancellationToken cancellationToken)
    {
        return _identityService.FindUserById(userId);
    }

    public Task<ApplicationUser?> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
    {
        return _identityService.FindActiveUserByUserName(normalizedUserName);
    }

    public Task SetPasswordHashAsync(ApplicationUser applicationUser, string? passwordHash, CancellationToken cancellationToken)
    {
        applicationUser.PasswordHash = passwordHash;
        return Task.CompletedTask;
    }

    public Task<string?> GetPasswordHashAsync(ApplicationUser applicationUser, CancellationToken cancellationToken)
    {
        return Task.FromResult(applicationUser.PasswordHash);
    }

    public Task<bool> HasPasswordAsync(ApplicationUser applicationUser, CancellationToken cancellationToken)
    {
        return Task.FromResult(!string.IsNullOrEmpty(applicationUser.PasswordHash));
    }

    public void Dispose()
    {
        _identityService.Dispose();
    }

    public Task SetSecurityStampAsync(ApplicationUser user, string stamp, CancellationToken cancellationToken)
    {
        user.SecurityStamp = stamp;
        return Task.CompletedTask;
    }

    public Task<string?> GetSecurityStampAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.SecurityStamp);
    }
}
