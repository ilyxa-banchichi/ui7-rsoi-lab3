using Microsoft.AspNetCore.Mvc;

namespace Gateway.API.Controllers;

[Route("")]
[ApiController]
public class ManageController : Controller
{
    [HttpGet("health")]
    public IActionResult Health()
    {
        return Ok();
    }
}