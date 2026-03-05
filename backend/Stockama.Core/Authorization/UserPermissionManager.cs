using Stockama.Core.Data;
using Stockama.Core.Exeptions;
using Stockama.Data.Domain;
using Stockama.Helper.Extensions;

namespace Stockama.Core.Authorization;

public sealed class UserPermissionManager : IUserPermissionManager
{
   private readonly IRepository<UserPermission> _userPermissionRepository;

   public UserPermissionManager(IRepository<UserPermission> userPermissionRepository)
   {
      _userPermissionRepository = userPermissionRepository;
   }

   public async Task AssignPermissionsAsync(Guid userId, IEnumerable<string> permissionCodes, Guid actorUserId)
   {
      if (userId.IsNullOrEmpty())
      {
         throw new CustomValidationException(nameof(userId));
      }

      var normalizedPermissionCodes = permissionCodes
         .Where(code => !string.IsNullOrWhiteSpace(code))
         .Select(code => code.Trim().ToLowerInvariant())
         .Distinct()
         .ToList();

      if (normalizedPermissionCodes.Count <= 0)
      {
         return;
      }

      var existingPermissions = await _userPermissionRepository.FilterActiveAsync(q =>
         q.UserId == userId && normalizedPermissionCodes.Contains(q.PermissionCode));

      var existingCodeSet = existingPermissions
         .Select(q => q.PermissionCode)
         .ToHashSet(StringComparer.OrdinalIgnoreCase);

      var newPermissions = normalizedPermissionCodes
         .Where(code => !existingCodeSet.Contains(code))
         .Select(code => new UserPermission
         {
            UserId = userId,
            PermissionCode = code
         })
         .ToList();

      if (newPermissions.Count <= 0)
      {
         return;
      }

      await _userPermissionRepository.CreateBulkAsync(newPermissions, actorUserId);
   }

   public async Task<IReadOnlyCollection<string>> GetPermissionsAsync(Guid userId)
   {
      if (userId.IsNullOrEmpty())
      {
         throw new CustomValidationException(nameof(userId));
      }

      var permissions = await _userPermissionRepository.FilterActiveAsync(q => q.UserId == userId);

      return permissions
         .Select(q => q.PermissionCode)
         .Distinct(StringComparer.OrdinalIgnoreCase)
         .ToList();
   }

   public async Task<bool> HasPermissionAsync(Guid userId, string permissionCode)
   {
      if (userId.IsNullOrEmpty())
      {
         throw new CustomValidationException(nameof(userId));
      }

      if (string.IsNullOrWhiteSpace(permissionCode))
      {
         throw new CustomValidationException(nameof(permissionCode));
      }

      var normalizedCode = permissionCode.Trim().ToLowerInvariant();

      return await _userPermissionRepository.AnyActiveAsync(q =>
         q.UserId == userId && q.PermissionCode == normalizedCode);
   }
}
