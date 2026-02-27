namespace Stockama.Core.Model.Response;

public class SuccessDataResponse<T> : BaseSuccessResponse<T>, IBaseDataResponse<T>
{
    public SuccessDataResponse(T data, int? total = null, string message = "") : base(data, total, message)
    {
    }
}
