using Stockama.Helper.Extensions;
using System.Net;

namespace Stockama.Core.Model.Response;

public class BaseErrorResponse<T> : IBaseResponse<T>
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; }
    public string Status { get; set; }
    public int? Total { get; set; }
    public T Data { get; set; }

    public BaseErrorResponse(HttpStatusCode status, string message = "")
    {
        Data = default;
        Total = null;
        IsSuccess = false;
        Status = status.ToIntString();
        Message = message;
    }
    public BaseErrorResponse(string status, string message = "")
    {
        Data = default;
        Total = null;
        IsSuccess = false;
        Status = status;
        Message = message;
    }

    public BaseErrorResponse(T data, HttpStatusCode status, string message = "")
    {
        Data = data;
        Total = null;
        IsSuccess = false;
        Status = status.ToIntString();
        Message = message;
    }
    public BaseErrorResponse(T data, string status, string message = "")
    {
        Data = data;
        Total = null;
        IsSuccess = false;
        Status = status;
        Message = message;
    }
}

