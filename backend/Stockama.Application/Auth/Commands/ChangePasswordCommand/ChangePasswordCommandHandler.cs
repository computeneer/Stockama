using LiteBus.Commands.Abstractions;
using Stockama.Core.Base;
using Stockama.Core.Data;
using Stockama.Core.Model.Response;
using Stockama.Core.Resources;
using Stockama.Core.Security;
using Stockama.Data.Domain;
using Stockama.Helper.Extensions;

namespace Stockama.Application.Auth.Commands.ChangePasswordCommand;

public sealed class ChangePasswordCommandHandler : BaseHandler, ICommandHandler<ChangePasswordCommand, IBaseBoolResponse>
{
   private readonly IRepository<User> _userRepository;
   private readonly IPasswordHasher _passwordHasher;

   public ChangePasswordCommandHandler(IResourceManager resourceManager, IRepository<User> userRepository, IPasswordHasher passwordHasher) : base(resourceManager)
   {
      _userRepository = userRepository;
      _passwordHasher = passwordHasher;
   }

   public async Task<IBaseBoolResponse> HandleAsync(ChangePasswordCommand message, CancellationToken cancellationToken = default)
   {
      if (message.UserId.IsNullOrEmpty())
      {
         return new ErrorBoolResponse("401", await T("api_error_unauthorized", message.LanguageId));
      }

      if (string.IsNullOrWhiteSpace(message.CurrentPassword) || string.IsNullOrWhiteSpace(message.NewPassword))
      {
         return new ErrorBoolResponse("400", await T("api_user_changepassword_passwordsdoesnotmatch", message.LanguageId));
      }

      var user = await _userRepository.GetActiveAsync(q => q.Id == message.UserId);
      if (user == null)
      {
         return new ErrorBoolResponse("404", await T("api_usernotfound", message.LanguageId));
      }

      if (!_passwordHasher.VerifyPassword(message.CurrentPassword, user.PasswordSalt, user.PasswordHash))
      {
         return new ErrorBoolResponse("400", await T("api_user_changepassword_invalidcurrentpassword", message.LanguageId));
      }

      var (passwordHash, passwordSalt) = _passwordHasher.HashPassword(message.NewPassword);
      user.PasswordHash = passwordHash;
      user.PasswordSalt = passwordSalt;
      user.MustChangePassword = false;
      user.OneTimePasswordUsed = true;
      user.OneTimePasswordExpiresAt = null;

      await _userRepository.UpdateAsync(user, message.UserId);

      return new SuccessBoolResponse(true);
   }
}
