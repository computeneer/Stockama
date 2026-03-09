using Stockama.Application.Auth.Models;
using Stockama.Core.Model.Response;
using Stockama.Helper.Api;

namespace Stockama.Application.Auth.Commands.RefreshAccessTokenCommand;

public sealed class RefreshAccessTokenCommand : BaseCommandRequest<IBaseDataResponse<LoginResponse>>
{
   public string ClientType { get; set; } = "web";
}
