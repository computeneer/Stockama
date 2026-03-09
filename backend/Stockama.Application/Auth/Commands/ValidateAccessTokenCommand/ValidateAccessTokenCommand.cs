using Stockama.Application.Auth.Models;
using Stockama.Core.Model.Response;
using Stockama.Helper.Api;

namespace Stockama.Application.Auth.Commands.ValidateAccessTokenCommand;

public sealed class ValidateAccessTokenCommand : BaseCommandRequest<IBaseDataResponse<ValidateTokenResponse>>
{
   public string ClientType { get; set; } = "web";
}
