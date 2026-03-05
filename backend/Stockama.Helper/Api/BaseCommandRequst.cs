using LiteBus.Commands.Abstractions;

namespace Stockama.Helper.Api;

public class BaseCommandRequest<T> : BaseRequest<T>, ICommand<T>
{ }