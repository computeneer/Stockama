using Stockama.Core.Model.Response;
using Stockama.Helper.Api;

namespace Stockama.Application.Auth.Commands.ChangePasswordCommand;

public sealed class ChangePasswordCommand : BaseCommandRequest<IBaseBoolResponse>
{
   public string CurrentPassword { get; set; } = string.Empty;
   public string NewPassword { get; set; } = string.Empty;
}
