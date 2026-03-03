using LiteBus.Commands.Abstractions;
using Stockama.Core.Base;
using Stockama.Core.Data;
using Stockama.Core.Model.Response;
using Stockama.Core.Resources;
using Stockama.Data.Domain;
using Stockama.Helper.Extensions;

namespace Stockama.Application.MessageTemplates.Commands.CreateMessageTemplateCommand;

public sealed class CreateMessageTemplateCommandHandler : BaseHandler, ICommandHandler<CreateMessageTemplateCommand, IBaseBoolResponse>
{
   private readonly IRepository<MessageTemplate> _messageTemplateRepository;
   private readonly IRepository<User> _userRepository;

   public CreateMessageTemplateCommandHandler(
      IResourceManager resourceManager,
      IRepository<MessageTemplate> messageTemplateRepository,
      IRepository<User> userRepository) : base(resourceManager)
   {
      _messageTemplateRepository = messageTemplateRepository;
      _userRepository = userRepository;
   }

   public async Task<IBaseBoolResponse> HandleAsync(CreateMessageTemplateCommand message, CancellationToken cancellationToken = default)
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

      var exists = await _messageTemplateRepository.AnyActiveAsync(q =>
         q.TemplateKey == normalizedTemplateKey && q.LanguageId == message.TemplateLanguageId);

      if (exists)
      {
         return new ErrorBoolResponse("409", "Message template already exists.");
      }

      var created = await _messageTemplateRepository.CreateBulkAsync(
      [
         new MessageTemplate
         {
            TemplateKey = normalizedTemplateKey,
            LanguageId = message.TemplateLanguageId,
            Subject = message.Subject,
            Body = message.Body
         }
      ],
      message.UserId);

      if (!created)
      {
         return new ErrorBoolResponse("500", "Message template could not be created.");
      }

      return new SuccessBoolResponse(true);
   }
}
