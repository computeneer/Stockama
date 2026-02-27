using System.Net;

namespace Stockama.Core.Model.Response;

public class ErrorGuidResponse : BaseErrorResponse<Guid?>, IBaseGuidResponse
{
    public ErrorGuidResponse(HttpStatusCode status, string message = "") : base(null, status, message)
    {
    }
    public ErrorGuidResponse(string status, string message = "") : base(null, status, message)
    {
    }
}
