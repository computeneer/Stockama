
using Stockama.Helper.Constants;
using Stockama.Helper.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;


namespace Stockama.Helper.Api;

[ApiController]
[Route("api/[controller]/[action]")]
public class BaseApiController : ControllerBase
{
   protected readonly IMediator _mediator;
   protected readonly IHttpContextAccessor _httpContextAccessor;
   protected Guid LanguageId;
   protected Guid UserId;

   public BaseApiController(IHttpContextAccessor httpContextAccessor, IMediator mediator)
   {
      _httpContextAccessor = httpContextAccessor;
      _mediator = mediator;
      LanguageId = SetLanguageId();
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

   protected async Task<object> Forward<T>(BaseRequest<T> baseRequest)
   {
      baseRequest.LanguageId = LanguageId;
      return await _mediator.Send(baseRequest) ?? new object();
   }

   protected async Task<object> ForwardAuth<T>(BaseRequest<T> baseRequest)
   {
      baseRequest.LanguageId = LanguageId;
      baseRequest.UserId = User.GetUserId();
      return await _mediator.Send(baseRequest) ?? new object();
   }
}