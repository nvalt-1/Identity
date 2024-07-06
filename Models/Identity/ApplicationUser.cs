using Microsoft.AspNetCore.Identity;

namespace Identity.Models.Identity;

public class ApplicationUser : IdentityUser<string>
{
    public override string Id { get; set; } = "";
    public override string? UserName { get; set; }
    public override string? NormalizedUserName { get; set; }
    public override string? PasswordHash { get; set; }
    public override string? SecurityStamp { get; set; }
}
