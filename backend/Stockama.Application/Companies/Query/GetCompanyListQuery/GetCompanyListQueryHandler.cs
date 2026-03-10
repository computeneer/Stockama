using LiteBus.Queries.Abstractions;
using Stockama.Application.Companies.Models;
using Stockama.Core.Base;
using Stockama.Core.Cache;
using Stockama.Core.Model.Response;
using Stockama.Core.Resources;

namespace Stockama.Application.Companies.Query.GetCompanyListQuery;

public sealed class GetCompanyListQueryHandler : BaseHandler, IQueryHandler<GetCompanyListQuery, IBaseListResponse<CompanyDto>>
{

   private readonly ICacheManager _cacheManager;

   public GetCompanyListQueryHandler(IResourceManager resourceManager, ICacheManager cacheManager) : base(resourceManager)
   {
      _cacheManager = cacheManager;
   }

   public async Task<IBaseListResponse<CompanyDto>> HandleAsync(GetCompanyListQuery message, CancellationToken cancellationToken = default)
   {
      var companies = await _cacheManager.GetCompanyCacheList();

      var dtoList = companies.Select(q => new CompanyDto
      {
         Id = q.Id,
         Name = q.Name,
         Description = q.Description ?? string.Empty,
         LogoUrl = q.LogoUrl ?? string.Empty,
         WebsiteUrl = q.WebsiteUrl ?? string.Empty,
         CompanyCode = q.CompanyCode
      }).ToList();

      return new SuccessListResponse<CompanyDto>(dtoList, dtoList.Count);
   }
}
