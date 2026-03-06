using FluentValidation;

namespace Stockama.Application.Auth.Commands.LoginCommand;

public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
{
   public LoginCommandValidator()
   {
      RuleFor(q => q.Username)
         .NotEmpty()
         .MaximumLength(31);

      RuleFor(q => q.Password)
         .NotEmpty()
         .MinimumLength(6);

      RuleFor(q => q.CompanyCode)
         .NotEmpty();

      RuleFor(q => q.ClientType)
         .NotEmpty()
         .Must(q => q.Equals("admin", StringComparison.OrdinalIgnoreCase) || q.Equals("web", StringComparison.OrdinalIgnoreCase))
         .WithMessage("ClientType must be admin or web.");
   }
}
