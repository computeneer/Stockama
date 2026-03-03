using LiteBus.Queries.Abstractions;
using Stockama.Application.Companies.Models;
using Stockama.Core.Base;
using Stockama.Core.Data;
using Stockama.Core.Model.Response;
using Stockama.Core.Resources;
using Stockama.Data.Domain;

namespace Stockama.Application.Companies.Query.GetCompanyListQuery;

public sealed class GetCompanyListQueryHandler : BaseHandler, IQueryHandler<GetCompanyListQuery, IBaseListResponse<CompanyDto>>
{

   private readonly IRepository<Company> _companyRepository;

   public GetCompanyListQueryHandler(IResourceManager resourceManager, IRepository<Company> companyRepository) : base(resourceManager)
   {
      _companyRepository = companyRepository;
   }

   public async Task<IBaseListResponse<CompanyDto>> HandleAsync(GetCompanyListQuery message, CancellationToken cancellationToken = default)
   {
      var companies = await _companyRepository.AllActiveAsync();

      var dtoList = companies.Select(q => new CompanyDto
      {
         Id = q.Id,
         Name = q.Name,
         Description = q.Description,
         LogoUrl = q.LogoUrl,
         WebsiteUrl = q.WebsiteUrl,
         CompanyCode = q.CompanyCode
      }).ToList();

      return new SuccessListResponse<CompanyDto>(dtoList, dtoList.Count);
   }
}
