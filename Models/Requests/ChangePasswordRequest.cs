namespace Identity.Models.Requests;

public class ChangePasswordRequest
{
    public string? OldPassword { get; init; }
    public string? NewPassword { get; init; }
}
