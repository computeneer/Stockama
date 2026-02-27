namespace Stockama.Core.Model.Response;

public class SuccessGuidResponse : BaseSuccessResponse<Guid?>, IBaseGuidResponse
{
    public SuccessGuidResponse(Guid data, int? total = null, string message = "") : base(data, total, message)
    {
    }
}
