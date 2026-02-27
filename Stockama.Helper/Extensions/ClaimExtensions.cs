using System.Security.Authentication;
using System.Security.Claims;

namespace Stockama.Helper.Extensions;

public static class ClaimExtensions
{
   public static Guid GetUserId(this ClaimsPrincipal claimsPrincipal)
   {
      if (claimsPrincipal.Identity is { IsAuthenticated: true }
         && claimsPrincipal.HasClaim(f => f.Type == ClaimTypes.NameIdentifier))
      {
         Guid.TryParse(claimsPrincipal.Claims.First(f => f.Type == ClaimTypes.NameIdentifier).Value, out var userId);
         return userId;
      }

      throw new AuthenticationException("UserId Not Found");
   }
}