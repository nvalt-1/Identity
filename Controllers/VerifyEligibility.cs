using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Controllers;

[ApiController]
[AllowAnonymous]
[Route("api/[controller]")]
public class VerifyEligibility : Controller
{

    [HttpPost]
    public IActionResult Verify()
    {
        // do actual verification here
        HttpContext.Session.SetString("personSerial", "1234");
        return Ok();
    }
}
