namespace Stockama.Core.Model.Response;

public interface IBaseResponse<T>
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; }
    public string Status { get; set; }
    public int? Total { get; set; }
    public T Data { get; set; }
}

