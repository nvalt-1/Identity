using Identity.Models.Identity;
using Identity.Models.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Controllers;

[ApiController]
[AllowAnonymous]
[Route("api/[controller]")]
public class LoginController : Controller
{
    private readonly SignInManager<ApplicationUser> _signInManager;

    public LoginController(SignInManager<ApplicationUser> signInManager)
    {
        _signInManager = signInManager;
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
        {
            return BadRequest();
        }

        var result = await _signInManager.PasswordSignInAsync(request.Username, request.Password, isPersistent: false, lockoutOnFailure: true);

        if (result.Succeeded)
        {
            return Ok("hooray");
        }

        if (result.RequiresTwoFactor)
        {
            return Ok("Requires two factor");
        }

        if (result.IsLockedOut)
        {
            return Unauthorized("Locked out");
        }

        return Unauthorized();
    }
}
