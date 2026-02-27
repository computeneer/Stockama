namespace Stockama.Core.Auth.Models;

public record TokenUser(Guid userId, string UserName, string Email, Guid CompanyId);
