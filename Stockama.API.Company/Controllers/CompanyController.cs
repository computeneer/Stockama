using LiteBus.Commands.Abstractions;
using LiteBus.Events.Abstractions;
using LiteBus.Queries.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Stockama.Helper.Api;

namespace Stockama.API.Company.Controllers;

public class CompanyController : BaseApiController
{
   public CompanyController(
      IHttpContextAccessor httpContextAccessor,
      ICommandMediator commandMediator,
      IQueryMediator queryMediator,
      IEventMediator eventMediator,
      IServiceProvider serviceProvider)
      : base(httpContextAccessor, commandMediator, queryMediator, eventMediator, serviceProvider)
   {
   }


}
