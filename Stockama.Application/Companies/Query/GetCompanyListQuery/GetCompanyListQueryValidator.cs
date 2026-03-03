using FluentValidation;

namespace Stockama.Application.Companies.Query.GetCompanyListQuery;

public sealed class GetCompanyListQueryValidator : AbstractValidator<GetCompanyListQuery>
{
   public GetCompanyListQueryValidator()
   {
      RuleFor(q => q.UserId)
         .NotEmpty();
   }
}
