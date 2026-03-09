using FluentValidation;

namespace Stockama.Application.Auth.Commands.SuperAdminLoginCommand;

public sealed class SuperAdminLoginCommandValidator : AbstractValidator<SuperAdminLoginCommand>
{
   public SuperAdminLoginCommandValidator()
   {
      RuleFor(q => q.Username)
         .NotEmpty()
         .MaximumLength(31);

      RuleFor(q => q.Password)
         .NotEmpty()
         .MinimumLength(6);

   }
}
