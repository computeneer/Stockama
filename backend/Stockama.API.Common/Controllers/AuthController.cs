using LiteBus.Commands.Abstractions;
using LiteBus.Events.Abstractions;
using LiteBus.Queries.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Stockama.Application.Auth.Commands.LoginCommand;
using Stockama.Application.Auth.Commands.RefreshAccessTokenCommand;
using Stockama.Application.Auth.Commands.RevokeAccessTokenCommand;
using Stockama.Application.Auth.Commands.ValidateAccessTokenCommand;
using Stockama.Helper.Api;

namespace Stockama.API.Common.Controllers;

public class AuthController : BaseApiController
{
   public AuthController(
      IHttpContextAccessor httpContextAccessor,
      ICommandMediator commandMediator,
      IQueryMediator queryMediator,
      IEventMediator eventMediator,
      IServiceProvider serviceProvider)
      : base(httpContextAccessor, commandMediator, queryMediator, eventMediator, serviceProvider)
   {
   }

   [HttpPost]
   public async Task<object> Login([FromBody] LoginCommand request)
   {
      return await ForwardCommand(request);
   }

   [HttpPost]
   public async Task<object> Refresh()
   {
      return await ForwardCommand(new RefreshAccessTokenCommand { ClientType = "web" });
   }

   [HttpPost]
   public async Task<object> Validate()
   {
      return await ForwardCommand(new ValidateAccessTokenCommand { ClientType = "web" });
   }

   [HttpPost]
   public async Task<object> Logout()
   {
      return await ForwardCommand(new RevokeAccessTokenCommand { ClientType = "web" });
   }

   [HttpPost]
   public async Task<object> Revoke()
   {
      return await ForwardCommand(new RevokeAccessTokenCommand { ClientType = "web" });
   }
}
