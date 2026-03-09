using Stockama.Core.Model.Response;
using Stockama.Helper.Api;

namespace Stockama.Application.Users.Commands.ResetUserOneTimePasswordCommand;

public sealed class ResetUserOneTimePasswordCommand : BaseCommandRequest<IBaseBoolResponse>
{
   public Guid TargetUserId { get; set; }
}
