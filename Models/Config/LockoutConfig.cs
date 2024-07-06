namespace Identity.Models.Config;

public class LockoutConfig
{
    public bool AllowedForNewUsers { get; } =  true;
    public int MaxFailedAccessAttempts { get; } =  5;
    public int DefaultLockoutMinutes { get; } =  5;
}
