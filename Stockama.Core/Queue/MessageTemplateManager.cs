using System.Text.RegularExpressions;
using Stockama.Core.Data;
using Stockama.Core.Exeptions;
using Stockama.Core.Queue.Models;
using Stockama.Data.Domain;
using Stockama.Helper.Constants;
using Stockama.Helper.Extensions;

namespace Stockama.Core.Queue;

public sealed class MessageTemplateManager : IMessageTemplateManager
{
   private static readonly Regex PlaceholderRegex = new(@"\{\{(?<key>[^{}]+)\}\}", RegexOptions.Compiled | RegexOptions.IgnoreCase);

   private readonly IRepository<MessageTemplate> _messageTemplateRepository;

   public MessageTemplateManager(IRepository<MessageTemplate> messageTemplateRepository)
   {
      _messageTemplateRepository = messageTemplateRepository;
   }

   public async Task<RenderedTemplateMessage> RenderAsync(string templateKey, Guid languageId, IReadOnlyDictionary<string, string> templateValues, CancellationToken cancellationToken = default)
   {
      if (string.IsNullOrWhiteSpace(templateKey))
      {
         throw new CustomValidationException(nameof(templateKey));
      }

      var normalizedTemplateKey = templateKey.Trim().ToLowerInvariant();
      var fallbackLanguageId = Guid.Parse(ApplicationContants.DefaultLanguageId);
      var requestedLanguageId = languageId.IsNullOrEmpty() ? fallbackLanguageId : languageId;

      var template = await _messageTemplateRepository.GetActiveAsync(q =>
         q.TemplateKey == normalizedTemplateKey && q.LanguageId == requestedLanguageId);

      if (template == null && requestedLanguageId != fallbackLanguageId)
      {
         template = await _messageTemplateRepository.GetActiveAsync(q =>
            q.TemplateKey == normalizedTemplateKey && q.LanguageId == fallbackLanguageId);
      }

      var (subjectTemplate, bodyTemplate) = template == null
         ? GetDefaultTemplate(normalizedTemplateKey)
         : (template.Subject, template.Body);

      var renderedSubject = FillTemplate(subjectTemplate, templateValues);
      var renderedBody = FillTemplate(bodyTemplate, templateValues);

      return new RenderedTemplateMessage(renderedSubject, renderedBody);
   }

   private static (string Subject, string Body) GetDefaultTemplate(string templateKey)
   {
      if (templateKey == MessageTemplateConstants.TenantAdminOneTimePasswordTemplateKey)
      {
         return (
            MessageTemplateConstants.TenantAdminOneTimePasswordDefaultSubject,
            MessageTemplateConstants.TenantAdminOneTimePasswordDefaultBody);
      }

      throw new HttpNotFoundExeption($"Message template not found: {templateKey}");
   }

   private static string FillTemplate(string template, IReadOnlyDictionary<string, string> templateValues)
   {
      if (string.IsNullOrEmpty(template))
      {
         return string.Empty;
      }

      if (templateValues.Count <= 0)
      {
         return template;
      }

      return PlaceholderRegex.Replace(template, match =>
      {
         var key = match.Groups["key"].Value.Trim();
         if (templateValues.TryGetValue(key, out var value))
         {
            return value ?? string.Empty;
         }

         return match.Value;
      });
   }
}
