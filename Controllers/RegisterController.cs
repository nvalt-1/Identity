using Identity.Models.Identity;
using Identity.Models.Requests;
using Identity.Models.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Controllers;

[ApiController]
[AllowAnonymous]
[Route("api/[controller]")]
public class RegisterController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;

    public RegisterController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        // Session must contain personSerial to register
        var personSerial = HttpContext.Session.GetString("personSerial");
        if (string.IsNullOrEmpty(personSerial))
        {
            return Unauthorized();
        }

        if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
        {
            var errors = new List<IdentityError>();
            if (string.IsNullOrEmpty(request.Username))
            {
                errors.Add(new IdentityError()
                {
                    Code = "InvalidUserName",
                    Description = "Empty or missing username is invalid."
                });
            }

            if (string.IsNullOrEmpty(request.Password))
            {
                errors.Add(new IdentityError()
                {
                    Code = "PasswordTooShort",
                    Description = "Empty or missing password is invalid."
                });
            }

            return BadRequest(new RegisterResponse()
            {
                Errors = errors
            });
        }

        var user = new ApplicationUser { UserName = request.Username, };
        var result = await _userManager.CreateAsync(user, request.Password);

        if (result.Succeeded)
        {
            return Ok();
        }

        return UnprocessableEntity(new RegisterResponse()
        {
            Errors = result.Errors.ToList()
        });
    }
}
