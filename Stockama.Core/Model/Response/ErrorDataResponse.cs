using System.Net;

namespace Stockama.Core.Model.Response;

public class ErrorDataResponse<T> : BaseErrorResponse<T>, IBaseDataResponse<T>
{
    public ErrorDataResponse(HttpStatusCode status, string message = "") : base(status, message)
    {
        Data = default(T);
    }
    public ErrorDataResponse(string status, string message = "") : base(status, message)
    {
        Data = default(T);
    }
}
