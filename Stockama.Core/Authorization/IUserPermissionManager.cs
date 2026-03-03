namespace Stockama.Core.Authorization;

public interface IUserPermissionManager
{
   Task AssignPermissionsAsync(Guid userId, IEnumerable<string> permissionCodes, Guid actorUserId);
   Task<IReadOnlyCollection<string>> GetPermissionsAsync(Guid userId);
   Task<bool> HasPermissionAsync(Guid userId, string permissionCode);
}
