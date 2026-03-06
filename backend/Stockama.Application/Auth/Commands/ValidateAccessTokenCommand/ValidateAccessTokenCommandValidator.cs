using FluentValidation;

namespace Stockama.Application.Auth.Commands.ValidateAccessTokenCommand;

public sealed class ValidateAccessTokenCommandValidator : AbstractValidator<ValidateAccessTokenCommand>
{
   public ValidateAccessTokenCommandValidator()
   {
      RuleFor(q => q.ClientType)
         .NotEmpty()
         .Must(q => q.Equals("admin", StringComparison.OrdinalIgnoreCase) || q.Equals("web", StringComparison.OrdinalIgnoreCase))
         .WithMessage("ClientType must be admin or web.");
   }
}
