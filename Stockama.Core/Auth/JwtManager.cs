using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Stockama.Core.Auth.Models;
using Stockama.Core.Data;
using Stockama.Core.Exeptions;
using Stockama.Data.Domain;
using Stockama.Helper;

namespace Stockama.Core.Auth;

public class JwtManager : IJwtManager
{
   private readonly JsonWebTokenHandler _tokenHandler;
   private readonly byte[] _jwtTokenKey;
   private readonly TokenValidationParameters _tokenValidationParameters;
   private readonly IHttpContextAccessor _httpContextAccessor;
   private readonly IRepository<User> _userRepository;


   public JwtManager(IHttpContextAccessor httpContextAccessor, IRepository<User> userRepository)
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
      _httpContextAccessor = httpContextAccessor;
      _userRepository = userRepository;
   }

   public async Task<JwtTokens> GenerateToken(TokenUser tokenUser)
   {
      var newRefreshToken = GenerateRefreshToken();
      var accessToken = GenerateAccessToken(tokenUser);

      var user = _userRepository.Get(q => q.Id == tokenUser.userId);

      if (user == null)
         throw new AuthenticationException("User not found");

      user?.RefreshToken = newRefreshToken;
      user?.RefreshTokenExpireDate = DateTime.UtcNow.AddDays(7);
      await _userRepository.UpdateBulkAsync([user], null);

      return new JwtTokens(accessToken.token, newRefreshToken, accessToken.validTo);
   }

   public async Task<JwtTokens> RefreshToken(string refreshToken)
   {
      if (_httpContextAccessor.HttpContext == null || _httpContextAccessor.HttpContext.User == null)
         throw new Exception("User is empty");

      var userId = Guid.Parse(_httpContextAccessor.HttpContext.User.Claims.First(q => q.Type == ClaimTypes.NameIdentifier).Value);

      var user = _userRepository.Get(q => q.Id == userId);

      if (user == null)
         throw new AuthenticationException("User not found");

      if (user.RefreshToken != refreshToken)
         throw new Exception("invalid refresh token");

      if (user.RefreshTokenExpireDate < DateTime.UtcNow)
         throw new Exception("refresh token expired");

      var newRefreshToken = GenerateRefreshToken();
      user.RefreshToken = newRefreshToken;
      user.RefreshTokenExpireDate = DateTime.UtcNow.AddDays(7);
      _userRepository.Update(user, null);

      var tokenUser = new TokenUser(user.Id, user.Username, user.Email, user.CompanyId);

      var (token, validTo) = GenerateAccessToken(tokenUser);

      return new JwtTokens(token, newRefreshToken, validTo);
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

   private (string token, DateTime validTo) GenerateAccessToken(TokenUser tokenUser)
   {
      var expires = DateTime.UtcNow.AddHours(1);
      var tokenDescriptor = new SecurityTokenDescriptor
      {
         Subject = new ClaimsIdentity(new[]
         {
            new Claim(ClaimTypes.NameIdentifier, tokenUser.userId.ToString()),
            new Claim(ClaimTypes.Email, tokenUser.Email),
            new Claim(nameof(tokenUser.UserName), tokenUser.UserName),
            new Claim(nameof(tokenUser.CompanyId), tokenUser.CompanyId.ToString())
         }),
         Issuer = "computeneer",
         Expires = expires,
         SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(_jwtTokenKey), SecurityAlgorithms.HmacSha256Signature),
         CompressionAlgorithm = CompressionAlgorithms.Deflate
      };

      var token = _tokenHandler.CreateToken(tokenDescriptor);
      return (token, expires);
   }

   private string GenerateRefreshToken()
   {
      var randomNumber = new byte[32];
      using var rng = RandomNumberGenerator.Create();
      rng.GetBytes(randomNumber);
      return Convert.ToBase64String(randomNumber);
   }

}