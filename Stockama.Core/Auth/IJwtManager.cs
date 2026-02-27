using Stockama.Core.Auth.Models;

namespace Stockama.Core.Auth;

public interface IJwtManager
{
   JwtTokens GenerateToken(TokenUser tokenUser);
   Task<bool> Validate(string token);
   Task<bool> Validate(JwtTokens jwtToken);
}