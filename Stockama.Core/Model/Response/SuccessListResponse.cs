namespace Stockama.Core.Model.Response;

public class SuccessListResponse<T> : BaseSuccessResponse<IEnumerable<T>>, IBaseListResponse<T>
{
    public SuccessListResponse(IEnumerable<T> data, int total, string message = "") : base(data, total, message)
    {
    }
}
