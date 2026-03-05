using FluentValidation;

namespace Stockama.Application.Users.Commands.ResetUserOneTimePasswordCommand;

public sealed class ResetUserOneTimePasswordCommandValidator : AbstractValidator<ResetUserOneTimePasswordCommand>
{
   public ResetUserOneTimePasswordCommandValidator()
   {
      RuleFor(q => q.UserId)
         .NotEmpty();

      RuleFor(q => q.TargetUserId)
         .NotEmpty()
         .NotEqual(q => q.UserId);
   }
}
