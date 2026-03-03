using Stockama.Core.Authorization.Models;

namespace Stockama.Core.Authorization;

public interface IJwtManager
{
   Task<JwtTokens> GenerateToken(TokenUser tokenUser);
   Task<JwtTokens> RefreshToken(string refreshToken);
   Task<bool> Validate(string token);
   Task<bool> Validate(JwtTokens jwtToken);
}