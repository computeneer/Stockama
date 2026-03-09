namespace Stockama.Helper.Api;

public abstract class BaseRequest<T> : IBaseRequest<T>
{
   public Guid UserId { get; set; }
   public Guid LanguageId { get; set; }
}