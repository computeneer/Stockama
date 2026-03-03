using Stockama.Core.Queue.Models;

namespace Stockama.Core.Queue;

public interface IMessageTemplateManager
{
   Task<RenderedTemplateMessage> RenderAsync(string templateKey, Guid languageId, IReadOnlyDictionary<string, string> templateValues, CancellationToken cancellationToken = default);
}
