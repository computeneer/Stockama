using Microsoft.AspNetCore.Mvc;

namespace Stockama.API.Common.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class TestController : Controller
{
   public async Task<IActionResult> HealthCheck()
   {
      // WORKING
      return Ok(DateTimeOffset.UtcNow);
   }
}