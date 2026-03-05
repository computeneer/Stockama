using LiteBus.Commands.Abstractions;
using Stockama.Core.Base;
using Stockama.Core.Data;
using Stockama.Core.Model.Response;
using Stockama.Core.Resources;
using Stockama.Data.Domain;
using Stockama.Helper.Extensions;

namespace Stockama.Application.MessageTemplates.Commands.UpdateMessageTemplateCommand;

public sealed class UpdateMessageTemplateCommandHandler : BaseHandler, ICommandHandler<UpdateMessageTemplateCommand, IBaseBoolResponse>
{
   private readonly IRepository<MessageTemplate> _messageTemplateRepository;
   private readonly IRepository<User> _userRepository;

   public UpdateMessageTemplateCommandHandler(
      IResourceManager resourceManager,
      IRepository<MessageTemplate> messageTemplateRepository,
      IRepository<User> userRepository) : base(resourceManager)
   {
      _messageTemplateRepository = messageTemplateRepository;
      _userRepository = userRepository;
   }

   public async Task<IBaseBoolResponse> HandleAsync(UpdateMessageTemplateCommand message, CancellationToken cancellationToken = default)
   {
      if (message.UserId.IsNullOrEmpty())
      {
         return new ErrorBoolResponse("401", await T("api_error_unauthorized", message.LanguageId));
      }

      var requesterUser = await _userRepository.GetActiveAsync(q => q.Id == message.UserId);
      if (requesterUser == null || !requesterUser.IsSuperAdmin)
      {
         return new ErrorBoolResponse("403", await T("api_error_forbidden", message.LanguageId));
      }

      var normalizedTemplateKey = message.TemplateKey.Trim().ToLowerInvariant();
      var template = await _messageTemplateRepository.GetActiveAsync(q =>
         q.TemplateKey == normalizedTemplateKey && q.LanguageId == message.TemplateLanguageId);

      if (template == null)
      {
         return new ErrorBoolResponse("404", "Message template not found.");
      }

      template.Subject = message.Subject;
      template.Body = message.Body;

      var updated = await _messageTemplateRepository.UpdateAsync(template, message.UserId);
      if (!updated)
      {
         return new ErrorBoolResponse("500", "Message template could not be updated.");
      }

      return new SuccessBoolResponse(true);
   }
}
