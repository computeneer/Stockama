using LiteBus.Commands.Abstractions;
using LiteBus.Events.Abstractions;
using LiteBus.Queries.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Stockama.Application.Users.Commands.ResetUserOneTimePasswordCommand;
using Stockama.Application.Users.Query.GetUserListQuery;
using Stockama.Helper.Api;

namespace Stockama.API.Admin.Controllers;

public class UserController : BaseApiController
{
   public UserController(
      IHttpContextAccessor httpContextAccessor,
      ICommandMediator commandMediator,
      IQueryMediator queryMediator,
      IEventMediator eventMediator,
      IServiceProvider serviceProvider)
      : base(httpContextAccessor, commandMediator, queryMediator, eventMediator, serviceProvider)
   {
   }

   [HttpGet]
   public async Task<object> List([FromQuery] GetUserListQuery request)
   {
      return await ForwardAuthQuery(request);
   }

   [HttpPost]
   public async Task<object> ResetOneTimePassword([FromBody] ResetUserOneTimePasswordCommand request)
   {
      return await ForwardAuthCommand(request);
   }
}
