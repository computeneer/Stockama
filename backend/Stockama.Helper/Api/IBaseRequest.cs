namespace Stockama.Helper.Api;

public interface IBaseRequest<T>
{
   Guid UserId { get; set; }
   Guid LanguageId { get; set; }
}