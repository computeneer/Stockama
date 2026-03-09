namespace Stockama.Core.Model.Response;

public class BaseSuccessResponse<T> : IBaseResponse<T>
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; }
    public string Status { get; set; }
    public int? Total { get; set; }
    public T Data { get; set; }

    public BaseSuccessResponse(T data, int? total, string message = "")
    {
        IsSuccess = true;
        Data = data;
        Total = total;
        Status = "200";
        Message = message;
    }
}

