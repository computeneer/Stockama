using System.Net;

namespace Stockama.Core.Model.Response;

public class ErrorBoolResponse : BaseErrorResponse<bool>, IBaseBoolResponse
{
    public ErrorBoolResponse(HttpStatusCode status, string message = "") : base(status, message)
    {
        Data = false;
    }
    public ErrorBoolResponse(string status, string message = "") : base(status, message)
    {
        Data = false;
    }
}
