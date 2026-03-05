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
   }
}
