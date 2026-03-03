using Ganss.Xss;
using LiteBus.Commands.Abstractions;
using LiteBus.Events.Abstractions;
using LiteBus.Queries.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Stockama.Application.MessageTemplates.Commands.CreateMessageTemplateCommand;
using Stockama.Application.MessageTemplates.Commands.UpdateMessageTemplateCommand;
using Stockama.Helper.Api;

namespace Stockama.API.Admin.Controllers;

public class MessageTemplateController : BaseApiController
{
   private readonly IHtmlSanitizer _htmlSanitizer;

   public MessageTemplateController(
      IHttpContextAccessor httpContextAccessor,
      ICommandMediator commandMediator,
      IQueryMediator queryMediator,
      IEventMediator eventMediator,
      IServiceProvider serviceProvider,
      IHtmlSanitizer htmlSanitizer)
      : base(httpContextAccessor, commandMediator, queryMediator, eventMediator, serviceProvider)
   {
      _htmlSanitizer = htmlSanitizer;
   }

   [HttpPost]
   public async Task<object> Create([FromBody] CreateMessageTemplateCommand request)
   {
      SanitizeRequest(request);
      return await ForwardAuthCommand(request);
   }

   [HttpPut]
   public async Task<object> Update([FromBody] UpdateMessageTemplateCommand request)
   {
      SanitizeRequest(request);
      return await ForwardAuthCommand(request);
   }

   private void SanitizeRequest(CreateMessageTemplateCommand request)
   {
      request.TemplateKey = request.TemplateKey?.Trim() ?? string.Empty;
      request.Subject = _htmlSanitizer.Sanitize(request.Subject ?? string.Empty).Trim();
      request.Body = _htmlSanitizer.Sanitize(request.Body ?? string.Empty).Trim();
   }

   private void SanitizeRequest(UpdateMessageTemplateCommand request)
   {
      request.TemplateKey = request.TemplateKey?.Trim() ?? string.Empty;
      request.Subject = _htmlSanitizer.Sanitize(request.Subject ?? string.Empty).Trim();
      request.Body = _htmlSanitizer.Sanitize(request.Body ?? string.Empty).Trim();
   }
}
