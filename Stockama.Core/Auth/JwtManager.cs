using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Stockama.Core.Auth.Models;
using Stockama.Helper;

namespace Stockama.Core.Auth;

public class JwtManager : IJwtManager
{
   private readonly JsonWebTokenHandler _tokenHandler;
   private readonly byte[] _jwtTokenKey;
   private readonly TokenValidationParameters _tokenValidationParameters;

   public JwtManager()
   {
      _tokenHandler = new JsonWebTokenHandler();
      _jwtTokenKey = Encoding.UTF8.GetBytes(EnvironmentVariables.JwtTokenKey);
      _tokenValidationParameters = new TokenValidationParameters
      {
         ValidIssuer = "computeneer",
         ValidateIssuer = true,
         ValidateLifetime = true,
         ValidateIssuerSigningKey = true,
         IssuerSigningKey = new SymmetricSecurityKey(_jwtTokenKey),
         ClockSkew = TimeSpan.Zero
      };
   }

   public JwtTokens GenerateToken(TokenUser tokenUser)
   {
      var expires = DateTime.UtcNow.AddDays(2);
      var tokenDescriptor = new SecurityTokenDescriptor
      {
         Subject = new ClaimsIdentity([
            new Claim(ClaimTypes.NameIdentifier, tokenUser.userId.ToString()),
            new Claim(ClaimTypes.Email, tokenUser.Email),
            new Claim(nameof(tokenUser.UserName), tokenUser.UserName),
            new Claim(nameof(tokenUser.CompanyId), tokenUser.CompanyId.ToString())
         ]),
         Issuer = "computeneer",
         Expires = expires,
         SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(_jwtTokenKey), algorithm: SecurityAlgorithms.HmacSha256Signature),
         CompressionAlgorithm = CompressionAlgorithms.Deflate
      };

      var token = _tokenHandler.CreateToken(tokenDescriptor);

      return new JwtTokens(token, token, expires);
   }

   public async Task<bool> Validate(string token)
   {
      var result = await
      _tokenHandler.ValidateTokenAsync(token, _tokenValidationParameters);

      return result.IsValid;
   }

   public async Task<bool> Validate(JwtTokens jwtToken)
   {
      return await Validate(jwtToken.AccessToken);
   }
}