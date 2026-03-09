using FluentValidation;

namespace Stockama.Application.MessageTemplates.Commands.CreateMessageTemplateCommand;

public sealed class CreateMessageTemplateCommandValidator : AbstractValidator<CreateMessageTemplateCommand>
{
   public CreateMessageTemplateCommandValidator()
   {
      RuleFor(q => q.UserId)
         .NotEmpty();

      RuleFor(q => q.TemplateKey)
         .NotEmpty()
         .MaximumLength(63)
         .Matches("^[a-zA-Z0-9._-]+$");

      RuleFor(q => q.TemplateLanguageId)
         .NotEmpty();

      RuleFor(q => q.Subject)
         .NotEmpty()
         .MaximumLength(255);

      RuleFor(q => q.Body)
         .NotEmpty();
   }
}
