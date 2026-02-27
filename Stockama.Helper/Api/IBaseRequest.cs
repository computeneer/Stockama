using MediatR;

namespace Stockama.Helper.Api;

public interface IBaseRequest<T> : IRequest<T>
{
   Guid UserId { get; set; }
   Guid LanguageId { get; set; }
}