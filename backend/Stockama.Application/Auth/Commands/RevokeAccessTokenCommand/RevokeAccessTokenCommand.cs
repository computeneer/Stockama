using Stockama.Core.Model.Response;
using Stockama.Helper.Api;

namespace Stockama.Application.Auth.Commands.RevokeAccessTokenCommand;

public sealed class RevokeAccessTokenCommand : BaseCommandRequest<IBaseBoolResponse>
{
   public string ClientType { get; set; } = "web";
}
