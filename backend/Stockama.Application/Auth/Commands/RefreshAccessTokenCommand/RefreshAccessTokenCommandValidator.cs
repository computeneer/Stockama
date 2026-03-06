using FluentValidation;

namespace Stockama.Application.Auth.Commands.RefreshAccessTokenCommand;

public sealed class RefreshAccessTokenCommandValidator : AbstractValidator<RefreshAccessTokenCommand>
{
   public RefreshAccessTokenCommandValidator()
   {
      RuleFor(q => q.ClientType)
         .NotEmpty()
         .Must(q => q.Equals("admin", StringComparison.OrdinalIgnoreCase) || q.Equals("web", StringComparison.OrdinalIgnoreCase))
         .WithMessage("ClientType must be admin or web.");
   }
}
