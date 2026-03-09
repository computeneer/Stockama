namespace Stockama.Core.Model.Response;

public class SuccessBoolResponse : BaseSuccessResponse<bool>, IBaseBoolResponse
{
    public SuccessBoolResponse(bool data = true, int? total = null, string message = "") : base(data, total, message)
    {
    }
}
