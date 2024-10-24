using Microsoft.AspNetCore.Mvc;

namespace Gateway.API.Controllers;

[Route("manage")]
[ApiController]
public class ManageController : Controller
{
    [HttpGet("health")]
    public IActionResult Health()
    {
        return Ok();
    }
}