using Stockama.Core.Authorization.Models;

namespace Stockama.Core.Authorization;

public interface IJwtManager
{
   Task<JwtTokens> GenerateToken(TokenUser tokenUser);
   Task<JwtTokens> GenerateToken(TokenUser tokenUser, string clientType);
   Task<JwtTokens> RefreshAccessToken(string accessToken, string clientType);
   Task<bool> RevokeAccessToken(string accessToken, string clientType);
   Task<bool> Validate(string token);
   Task<bool> Validate(string token, string expectedClientType);
   Task<bool> Validate(JwtTokens jwtToken);
}
