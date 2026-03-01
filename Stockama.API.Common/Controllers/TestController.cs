using LiteBus.Commands.Abstractions;
using LiteBus.Events.Abstractions;
using LiteBus.Queries.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Stockama.Application.Auth.Commands.LoginCommand;
using Stockama.Core.Security;
using Stockama.Helper.Api;

namespace Stockama.API.Common.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class TestController : BaseApiController
{
   public TestController(IHttpContextAccessor httpContextAccessor, ICommandMediator commandMediator, IQueryMediator queryMediator, IEventMediator eventMediator) : base(httpContextAccessor, commandMediator, queryMediator, eventMediator)
   {
   }

   [HttpGet]
   public async Task<IActionResult> HealthCheck()
   {
      // WORKING
      return Ok(DateTimeOffset.UtcNow);
   }

   [HttpGet]
   public async Task<IActionResult> HashPassword([FromQuery] string password)
   {
      var passwordHasher = new PasswordHasher();
      var (hash, salt) = passwordHasher.HashPassword(password);

      var validResponse = passwordHasher.VerifyPassword(password, salt, hash);
      var invalidResponse = passwordHasher.VerifyPassword(password + "invalid", salt, hash);

      return Ok(new { Hash = hash, Salt = salt, ValidResponse = validResponse, InvalidResponse = invalidResponse });
   }

   [HttpGet]
   public async Task<IActionResult> Login([FromQuery] LoginCommand loginCommand)
   {
      var result = await ForwardCommand(loginCommand);
      return Ok(result);
   }
}