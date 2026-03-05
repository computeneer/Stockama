using FluentValidation;

namespace Stockama.Application.Auth.Commands.ChangePasswordCommand;

public sealed class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
{
   public ChangePasswordCommandValidator()
   {
      RuleFor(q => q.UserId)
         .NotEmpty();

      RuleFor(q => q.CurrentPassword)
         .NotEmpty()
         .MinimumLength(6);

      RuleFor(q => q.NewPassword)
         .NotEmpty()
         .MinimumLength(6)
         .NotEqual(q => q.CurrentPassword);
   }
}
