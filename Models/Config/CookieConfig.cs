namespace Identity.Models.Config;

public class CookieConfig
{
    public int ExpireTimeMinutes { get; } = 10;
    public bool SlidingExpiration { get; } = true;
}
