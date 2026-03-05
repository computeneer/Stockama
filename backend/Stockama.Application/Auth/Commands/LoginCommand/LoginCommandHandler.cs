using LiteBus.Commands.Abstractions;
using Stockama.Core.Authorization;
using Stockama.Application.Authorization.Models;
using Stockama.Core.Base;
using Stockama.Core.Data;
using Stockama.Core.Model.Response;
using Stockama.Core.Resources;
using Stockama.Core.Security;
using Stockama.Data.Domain;
using Stockama.Core.Authorization.Models;

namespace Stockama.Application.Authorization.Commands.LoginCommand;

public sealed class LoginCommandHandler : BaseHandler, ICommandHandler<LoginCommand, IBaseDataResponse<LoginResponse>>
{
    private readonly IRepository<User> _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtManager _jwtManager;

    public LoginCommandHandler(
        IResourceManager resourceManager,
        IRepository<User> userRepository,
        IPasswordHasher passwordHasher,
        IJwtManager jwtManager) : base(resourceManager)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtManager = jwtManager;
    }

    public async Task<IBaseDataResponse<LoginResponse>> HandleAsync(LoginCommand message, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetActiveAsync(u => u.Username.ToLower() == message.Username.ToLower());
        if (user == null)
        {
            return new ErrorDataResponse<LoginResponse>("401", await T("api_auth_usernotfound", message.LanguageId));
        }

        if (!_passwordHasher.VerifyPassword(message.Password, user.PasswordSalt, user.PasswordHash))
        {
            return new ErrorDataResponse<LoginResponse>("401", await T("api_auth_usernotfound", message.LanguageId));
        }

        var requirePasswordChange = user.MustChangePassword;

        if (requirePasswordChange)
        {
            if (user.OneTimePasswordExpiresAt == null || user.OneTimePasswordExpiresAt <= DateTimeOffset.UtcNow)
            {
                return new ErrorDataResponse<LoginResponse>("401", await T("api_user_onetimepassword_expired", message.LanguageId));
            }

            if (user.OneTimePasswordUsed)
            {
                return new ErrorDataResponse<LoginResponse>("401", await T("api_user_onetimepassword_used", message.LanguageId));
            }

            user.OneTimePasswordUsed = true;
            await _userRepository.UpdateAsync(user, user.Id);
        }

        var tokenUser = new TokenUser(
            user.Id,
            user.Username,
            user.Email,
            user.CompanyId,
            user.IsSuperAdmin,
            user.IsTenantAdmin,
            user.MustChangePassword);
        var tokens = await _jwtManager.GenerateToken(tokenUser);

        var response = new LoginResponse(tokens.AccessToken, tokens.ValidTo, requirePasswordChange);

        return new SuccessDataResponse<LoginResponse>(response);
    }
}
