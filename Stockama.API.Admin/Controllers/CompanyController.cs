using LiteBus.Commands.Abstractions;
using LiteBus.Events.Abstractions;
using LiteBus.Queries.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Stockama.Application.Companies.Command.CreateCompanyCommand;
using Stockama.Application.Companies.Query.GetCompanyListQuery;
using Stockama.Helper.Api;

namespace Stockama.API.Admin.Controllers;

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

   [HttpPost]
   public async Task<object> Create([FromBody] CreateCompanyCommand request)
   {
      return await ForwardAuthCommand(request);
   }

   [HttpGet]
   public async Task<object> List([FromQuery] GetCompanyListQuery request)
   {
      return await ForwardAuthQuery(request);
   }
}
