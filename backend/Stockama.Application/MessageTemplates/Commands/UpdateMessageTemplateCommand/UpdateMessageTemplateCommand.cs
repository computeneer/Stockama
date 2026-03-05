using Stockama.Core.Model.Response;
using Stockama.Helper.Api;

namespace Stockama.Application.MessageTemplates.Commands.UpdateMessageTemplateCommand;

public sealed class UpdateMessageTemplateCommand : BaseCommandRequest<IBaseBoolResponse>
{
   public string TemplateKey { get; set; } = string.Empty;
   public Guid TemplateLanguageId { get; set; }
   public string Subject { get; set; } = string.Empty;
   public string Body { get; set; } = string.Empty;
}
