
using Stockama.Helper.Constants;
using Stockama.Helper.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using LiteBus.Commands.Abstractions;
using LiteBus.Queries.Abstractions;
using LiteBus.Events.Abstractions;

namespace Stockama.Helper.Api;

[ApiController]
[Route("api/[controller]/[action]")]
public class BaseApiController : ControllerBase
{
   protected readonly ICommandMediator _commandMediator;
   protected readonly IQueryMediator _queryMediator;
   protected readonly IEventMediator _eventMediator;

   protected readonly IHttpContextAccessor _httpContextAccessor;
   protected Guid LanguageId;
   protected Guid UserId;

   public BaseApiController(IHttpContextAccessor httpContextAccessor, ICommandMediator commandMediator, IQueryMediator queryMediator, IEventMediator eventMediator)
   {
      _httpContextAccessor = httpContextAccessor;
      LanguageId = SetLanguageId();
      _commandMediator = commandMediator;
      _queryMediator = queryMediator;
      _eventMediator = eventMediator;
   }

   protected Guid SetLanguageId()
   {
      if (_httpContextAccessor is not null && _httpContextAccessor.HttpContext is not null)
      {
         var isExists = _httpContextAccessor.HttpContext.Request.Headers.TryGetValue("LanguageId", out var languageId);
         if (isExists)
         {
            Guid.TryParse(languageId, out var parsedLanguageId);
            return parsedLanguageId;
         }
         return Guid.Parse(ApplicationContants.DefaultLanguageId);
      }
      return Guid.Parse(ApplicationContants.DefaultLanguageId);
   }

   protected async Task<object> ForwardCommand<T>(BaseCommandRequest<T> baseRequest)
   {
      baseRequest.LanguageId = LanguageId;
      return await _commandMediator.SendAsync(baseRequest) ?? new object();
   }

   protected async Task<object> ForwardAuthCommand<T>(BaseCommandRequest<T> baseRequest)
   {
      baseRequest.LanguageId = LanguageId;
      baseRequest.UserId = User.GetUserId();
      return await _commandMediator.SendAsync(baseRequest) ?? new object();
   }

   protected async Task<object> ForwardQuery<T>(BaseQueryRequest<T> baseRequest)
   {
      baseRequest.LanguageId = LanguageId;
      return await _queryMediator.QueryAsync(baseRequest) ?? new object();
   }

   protected async Task<object> ForwardAuthQuery<T>(BaseQueryRequest<T> baseRequest)
   {
      baseRequest.LanguageId = LanguageId;
      baseRequest.UserId = User.GetUserId();
      return await _queryMediator.QueryAsync(baseRequest) ?? new object();
   }
}