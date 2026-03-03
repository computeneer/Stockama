using Stockama.Application.Authorization.Models;
using Stockama.Core.Model.Response;
using Stockama.Helper.Api;

namespace Stockama.Application.Authorization.Commands.LoginCommand;

public sealed class LoginCommand : BaseCommandRequest<IBaseDataResponse<LoginResponse>>
{
   public string Username { get; set; } = string.Empty;
   public string Password { get; set; } = string.Empty;
}