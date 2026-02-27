using MediatR;

namespace Stockama.Helper.Api;

public class BaseRequest<T> : IRequest<T>
{
   public Guid UserId { get; set; }
   public Guid LanguageId { get; set; }
}