using LiteBus.Commands.Abstractions;
using Stockama.Application.Auth.Models;
using Stockama.Core.Auth;
using Stockama.Core.Data;
using Stockama.Core.Model.Response;
using Stockama.Core.Security;
using Stockama.Data.Domain;

namespace Stockama.Application.Auth.Commands.LoginCommand;

public sealed class LoginCommandHandler : ICommandHandler<LoginCommand, IBaseDataResponse<LoginResponse>>
{
    private readonly IRepository<User> _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtManager _jwtManager;

    public LoginCommandHandler(IRepository<User> userRepository, IPasswordHasher passwordHasher, IJwtManager jwtManager)
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
            return new ErrorDataResponse<LoginResponse>("Invalid username or password");
        }

        if (!_passwordHasher.VerifyPassword(message.Password, user.PasswordSalt, user.PasswordHash))
        {
            return new ErrorDataResponse<LoginResponse>("Invalid username or password");
        }

        var tokenUser = new Core.Auth.Models.TokenUser(user.Id, user.Username, user.Email, user.CompanyId);
        var tokens = await _jwtManager.GenerateToken(tokenUser);

        var response = new LoginResponse(tokens.AccessToken, tokens.ValidTo);

        return new SuccessDataResponse<LoginResponse>(response);
    }
}