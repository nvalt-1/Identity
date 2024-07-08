using Microsoft.AspNetCore.Identity;

namespace Identity.Models.Responses;

public class RegisterResponse
{
    public List<IdentityError>? Errors { get; set; }
}
