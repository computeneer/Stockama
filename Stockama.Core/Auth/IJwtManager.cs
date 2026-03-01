using Stockama.Core.Auth.Models;

namespace Stockama.Core.Auth;

public interface IJwtManager
{
   Task<JwtTokens> GenerateToken(TokenUser tokenUser);
   Task<JwtTokens> RefreshToken(string refreshToken);
   Task<bool> Validate(string token);
   Task<bool> Validate(JwtTokens jwtToken);
}