using Stockama.Application.Companies.Models;
using Stockama.Core.Model.Response;
using Stockama.Helper.Api;

namespace Stockama.Application.Companies.Query.GetCompanyListQuery;

public sealed class GetCompanyListQuery : BaseQueryRequest<IBaseListResponse<CompanyDto>>
{

}