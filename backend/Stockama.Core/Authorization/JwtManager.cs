using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Stockama.Core.Authorization.Models;
using Stockama.Core.Data;
using Stockama.Core.Exeptions;
using Stockama.Data.Domain;
using Stockama.Helper;

namespace Stockama.Core.Authorization;

public class JwtManager : IJwtManager
{
   private const int RefreshTokenExpirationDays = 30;
   private readonly JsonWebTokenHandler _tokenHandler;
   private readonly byte[] _jwtTokenKey;
   private readonly TokenValidationParameters _tokenValidationParameters;
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
         ValidateAudience = false,
         ValidateActor = false,
         IssuerSigningKey = new SymmetricSecurityKey(_jwtTokenKey),
         ClockSkew = TimeSpan.Zero
      };
      _ = httpContextAccessor;
      _userRepository = userRepository;
   }

   public async Task<JwtTokens> GenerateToken(TokenUser tokenUser)
   {
      return await GenerateToken(tokenUser, "web");
   }

   public async Task<JwtTokens> GenerateToken(TokenUser tokenUser, string clientType)
   {
      var normalizedClientType = NormalizeClientType(clientType);
      var (accessToken, validTo) = GenerateAccessToken(tokenUser, normalizedClientType);
      var newRefreshToken = GenerateRefreshToken(accessToken, normalizedClientType);

      var user = _userRepository.Get(q => q.Id == tokenUser.userId);
      var existingUser = user ?? throw new AuthenticationException("User not found");

      existingUser.RefreshToken = newRefreshToken;
      existingUser.RefreshTokenExpireDate = DateTime.UtcNow.AddDays(RefreshTokenExpirationDays);
      await _userRepository.UpdateBulkAsync([existingUser], null);

      return new JwtTokens(accessToken, newRefreshToken, validTo);
   }

   public async Task<JwtTokens> RefreshAccessToken(string accessToken, string clientType)
   {
      var normalizedClientType = NormalizeClientType(clientType);
      var claimsIdentity = await ValidateAccessTokenInternal(accessToken, normalizedClientType, validateLifetime: false);
      var userId = GetUserIdFromClaims(claimsIdentity);
      if (userId == Guid.Empty)
      {
         throw new AuthenticationException("invalid access token");
      }

      var user = _userRepository.Get(q => q.Id == userId);
      var existingUser = user ?? throw new AuthenticationException("User not found");

      if (!HasUsableRefreshToken(existingUser))
      {
         await InvalidateUserTokens(existingUser);
         throw new AuthenticationException("refresh token expired");
      }

      if (!MatchesClientType(existingUser.RefreshToken!, normalizedClientType)
         || !IsRefreshTokenBoundToAccessToken(existingUser.RefreshToken!, accessToken, normalizedClientType))
      {
         await InvalidateUserTokens(existingUser);
         throw new AuthenticationException("invalid token session");
      }

      var tokenUser = new TokenUser(
         existingUser.Id,
         existingUser.Username,
         existingUser.Email,
         existingUser.CompanyId,
         existingUser.IsSuperAdmin,
         existingUser.IsTenantAdmin,
         existingUser.MustChangePassword);

      var (token, validTo) = GenerateAccessToken(tokenUser, normalizedClientType);
      var newRefreshToken = GenerateRefreshToken(token, normalizedClientType);

      existingUser.RefreshToken = newRefreshToken;
      existingUser.RefreshTokenExpireDate = DateTime.UtcNow.AddDays(RefreshTokenExpirationDays);
      await _userRepository.UpdateBulkAsync([existingUser], null);

      return new JwtTokens(token, newRefreshToken, validTo);
   }

   public async Task<bool> RevokeAccessToken(string accessToken, string clientType)
   {
      var normalizedClientType = NormalizeClientType(clientType);

      ClaimsIdentity claimsIdentity;
      try
      {
         claimsIdentity = await ValidateAccessTokenInternal(accessToken, normalizedClientType, validateLifetime: false);
      }
      catch
      {
         return false;
      }

      var userId = GetUserIdFromClaims(claimsIdentity);
      if (userId == Guid.Empty)
      {
         return false;
      }

      var user = _userRepository.Get(q => q.Id == userId);
      if (user == null || string.IsNullOrWhiteSpace(user.RefreshToken))
      {
         return false;
      }

      await InvalidateUserTokens(user);
      return true;
   }

   public async Task<bool> Validate(string token)
   {
      var result = await _tokenHandler.ValidateTokenAsync(token, _tokenValidationParameters);
      if (!result.IsValid || result.ClaimsIdentity == null)
      {
         return false;
      }

      var userId = GetUserIdFromClaims(result.ClaimsIdentity);
      if (userId == Guid.Empty)
      {
         return false;
      }

      var user = _userRepository.Get(q => q.Id == userId);
      if (!HasUsableRefreshToken(user))
      {
         return false;
      }

      var tokenClientType = NormalizeClientType(result.ClaimsIdentity.FindFirst("client_type")?.Value ?? "web");
      return MatchesClientType(user!.RefreshToken!, tokenClientType)
         && IsRefreshTokenBoundToAccessToken(user.RefreshToken!, token, tokenClientType);
   }

   public async Task<bool> Validate(string token, string expectedClientType)
   {
      var normalizedClientType = NormalizeClientType(expectedClientType);
      var parameters = BuildTokenValidationParameters(normalizedClientType, validateLifetime: true);

      var result = await _tokenHandler.ValidateTokenAsync(token, parameters);
      if (!result.IsValid || result.ClaimsIdentity == null)
      {
         return false;
      }

      var userId = GetUserIdFromClaims(result.ClaimsIdentity);
      if (userId == Guid.Empty)
      {
         return false;
      }

      var user = _userRepository.Get(q => q.Id == userId);
      if (!HasUsableRefreshToken(user))
      {
         return false;
      }

      return MatchesClientType(user!.RefreshToken!, normalizedClientType)
         && IsRefreshTokenBoundToAccessToken(user.RefreshToken!, token, normalizedClientType);
   }

   public async Task<bool> Validate(JwtTokens jwtToken)
   {
      return await Validate(jwtToken.AccessToken);
   }

   private (string token, DateTime validTo) GenerateAccessToken(TokenUser tokenUser, string clientType)
   {
      var expires = DateTime.UtcNow.AddHours(1);
      var normalizedClientType = NormalizeClientType(clientType);
      var tokenDescriptor = new SecurityTokenDescriptor
      {
         Subject = new ClaimsIdentity(new[]
         {
            new Claim(ClaimTypes.NameIdentifier, tokenUser.userId.ToString()),
            new Claim(ClaimTypes.Email, tokenUser.Email),
            new Claim(nameof(tokenUser.UserName), tokenUser.UserName),
            new Claim(nameof(tokenUser.CompanyId), tokenUser.CompanyId.ToString()),
            new Claim(nameof(tokenUser.IsSuperAdmin), tokenUser.IsSuperAdmin.ToString()),
            new Claim(nameof(tokenUser.IsTenantAdmin), tokenUser.IsTenantAdmin.ToString()),
            new Claim(nameof(tokenUser.MustChangePassword), tokenUser.MustChangePassword.ToString()),
            new Claim("client_type", normalizedClientType)
         }),
         Issuer = "computeneer",
         Audience = normalizedClientType,
         Expires = expires,
         SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(_jwtTokenKey), SecurityAlgorithms.HmacSha256Signature),
         CompressionAlgorithm = CompressionAlgorithms.Deflate
      };

      var token = _tokenHandler.CreateToken(tokenDescriptor);
      return (token, expires);
   }

   private async Task<ClaimsIdentity> ValidateAccessTokenInternal(string accessToken, string expectedClientType, bool validateLifetime)
   {
      var parameters = BuildTokenValidationParameters(expectedClientType, validateLifetime);
      var result = await _tokenHandler.ValidateTokenAsync(accessToken, parameters);
      if (!result.IsValid || result.ClaimsIdentity == null)
      {
         throw new AuthenticationException("invalid access token");
      }

      return result.ClaimsIdentity;
   }

   private TokenValidationParameters BuildTokenValidationParameters(string expectedClientType, bool validateLifetime)
   {
      var parameters = _tokenValidationParameters.Clone();
      parameters.ValidateAudience = true;
      parameters.ValidAudience = expectedClientType;
      parameters.ValidateLifetime = validateLifetime;
      return parameters;
   }

   private static Guid GetUserIdFromClaims(ClaimsIdentity claimsIdentity)
   {
      var userIdValue = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      return Guid.TryParse(userIdValue, out var userId) ? userId : Guid.Empty;
   }

   private static bool HasUsableRefreshToken(User? user)
   {
      return user is not null
         && !string.IsNullOrWhiteSpace(user.RefreshToken)
         && user.RefreshTokenExpireDate.HasValue
         && user.RefreshTokenExpireDate.Value > DateTime.UtcNow;
   }

   private async Task InvalidateUserTokens(User user)
   {
      user.RefreshToken = null;
      user.RefreshTokenExpireDate = null;
      await _userRepository.UpdateBulkAsync([user], null);
   }

   private string GenerateRefreshToken(string accessToken, string clientType)
   {
      var randomNumber = new byte[32];
      using var rng = RandomNumberGenerator.Create();
      rng.GetBytes(randomNumber);
      var randomToken = Base64UrlEncoder.Encode(randomNumber);
      var accessTokenHash = ComputeAccessTokenHash(accessToken);
      return $"{NormalizeClientType(clientType)}.{accessTokenHash}.{randomToken}";
   }

   private static string ComputeAccessTokenHash(string accessToken)
   {
      var hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(accessToken));
      return Base64UrlEncoder.Encode(hashBytes);
   }

   private static string NormalizeClientType(string clientType)
   {
      return string.Equals(clientType, "admin", StringComparison.OrdinalIgnoreCase) ? "admin" : "web";
   }

   private static bool MatchesClientType(string refreshToken, string expectedClientType)
   {
      return refreshToken.StartsWith($"{expectedClientType}.", StringComparison.Ordinal);
   }

   private static bool IsRefreshTokenBoundToAccessToken(string refreshToken, string accessToken, string expectedClientType)
   {
      var accessTokenHash = ComputeAccessTokenHash(accessToken);
      return refreshToken.StartsWith($"{expectedClientType}.{accessTokenHash}.", StringComparison.Ordinal);
   }

}
