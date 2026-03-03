namespace Stockama.Application.Authorization.Models;

public record LoginResponse(string AccessToken, DateTime ValidTo, bool RequirePasswordChange);
