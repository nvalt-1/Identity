using Identity.Models.Identity;
using Identity.Models.Requests;
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
        if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
        {
            return BadRequest();
        }

        var user = new ApplicationUser { UserName = request.Username, };
        var result = await _userManager.CreateAsync(user, request.Password);
        if (result.Succeeded)
        {
            return Ok($"Registered {request.Username}");
        }

        #if DEBUG
        return BadRequest(result.Errors);
        #else
        return BadRequest();
        #endif
    }
}
