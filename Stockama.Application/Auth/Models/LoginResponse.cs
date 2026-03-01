namespace Stockama.Application.Auth.Models;

public record LoginResponse(string AccessToken, DateTime ValidTo);