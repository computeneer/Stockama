using Stockama.Application.Auth.Models;
using Stockama.Core.Model.Response;
using Stockama.Helper.Api;

namespace Stockama.Application.Auth.Commands.LoginCommand;

public sealed class LoginCommand : BaseCommandRequest<IBaseDataResponse<LoginResponse>>
{
   public string Username { get; set; } = string.Empty;
   public string Password { get; set; } = string.Empty;
   public string CompanyCode { get; set; } = string.Empty;
   public string ClientType { get; set; } = "web";
}
