using FluentValidation;

namespace Stockama.Application.Companies.Command.CreateCompanyCommand;

public sealed class CreateCompanyCommandValidator : AbstractValidator<CreateCompanyCommand>
{
   public CreateCompanyCommandValidator()
   {
      RuleFor(q => q.UserId)
         .NotEmpty();

      RuleFor(q => q.Name)
         .NotEmpty()
         .MaximumLength(127);

      RuleFor(q => q.Description)
         .MaximumLength(255);

      RuleFor(q => q.CompanyCode)
         .NotEmpty()
         .MaximumLength(15)
         .Matches("^[a-zA-Z0-9._-]+$");

      RuleFor(q => q.AdminUsername)
         .NotEmpty()
         .MaximumLength(31);

      RuleFor(q => q.AdminFirstName)
         .NotEmpty()
         .MaximumLength(63);

      RuleFor(q => q.AdminLastName)
         .NotEmpty()
         .MaximumLength(63);

      RuleFor(q => q.AdminEmail)
         .EmailAddress()
         .When(q => !string.IsNullOrWhiteSpace(q.AdminEmail));
   }
}
