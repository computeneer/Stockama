using System.Net;

namespace Stockama.Core.Model.Response;

public class ErrorListResponse<T> : BaseErrorResponse<IEnumerable<T>>, IBaseListResponse<T>
{
    public ErrorListResponse(HttpStatusCode status, string message = "") : base(status, message)
    {
    }
    public ErrorListResponse(string status, string message = "") : base(status, message)
    {
    }
}
