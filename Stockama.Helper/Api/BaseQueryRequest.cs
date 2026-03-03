using LiteBus.Queries.Abstractions;

namespace Stockama.Helper.Api;

public class BaseQueryRequest<T> : BaseRequest<T>, IQuery<T>
{

}