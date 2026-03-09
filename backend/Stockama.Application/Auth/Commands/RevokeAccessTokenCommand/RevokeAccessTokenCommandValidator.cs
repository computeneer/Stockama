using FluentValidation;

namespace Stockama.Application.Auth.Commands.RevokeAccessTokenCommand;

public sealed class RevokeAccessTokenCommandValidator : AbstractValidator<RevokeAccessTokenCommand>
{
   public RevokeAccessTokenCommandValidator()
   {
      RuleFor(q => q.ClientType)
         .NotEmpty()
         .Must(q => q.Equals("admin", StringComparison.OrdinalIgnoreCase) || q.Equals("web", StringComparison.OrdinalIgnoreCase))
         .WithMessage("ClientType must be admin or web.");
   }
}
