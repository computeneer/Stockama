using Stockama.Application.Auth.Models;
using Stockama.Core.Model.Response;
using Stockama.Helper.Api;

namespace Stockama.Application.Auth.Commands.SuperAdminLoginCommand;

public sealed class SuperAdminLoginCommand : BaseCommandRequest<IBaseDataResponse<LoginResponse>>
{
   public string Username { get; set; } = string.Empty;
   public string Password { get; set; } = string.Empty;
}