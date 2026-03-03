using FluentValidation;

namespace Stockama.Application.Users.Query.GetUserListQuery;

public sealed class GetUserListQueryValidator : AbstractValidator<GetUserListQuery>
{
   public GetUserListQueryValidator()
   {
      RuleFor(q => q.UserId)
         .NotEmpty();

      RuleFor(q => q.CompanyId)
         .Must(q => q == null || q != Guid.Empty)
         .WithMessage("CompanyId is not valid.");
   }
}
