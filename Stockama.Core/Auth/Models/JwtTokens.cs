namespace Stockama.Core.Auth.Models;

public record JwtTokens(string AccessToken, string RefreshToken, DateTime ValidTo);
