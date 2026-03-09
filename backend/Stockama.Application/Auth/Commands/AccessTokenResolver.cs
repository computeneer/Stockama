using Microsoft.AspNetCore.Http;
using Stockama.Core.Exeptions;

namespace Stockama.Application.Auth.Commands;

internal static class AccessTokenResolver
{
   public static string ResolveFromAuthorizationHeader(IHttpContextAccessor httpContextAccessor)
   {
      var authHeader = httpContextAccessor.HttpContext?.Request.Headers.Authorization.ToString();
      if (!string.IsNullOrWhiteSpace(authHeader) && authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
      {
         var token = authHeader["Bearer ".Length..].Trim();
         if (!string.IsNullOrWhiteSpace(token))
         {
            return token;
         }
      }

      throw new AuthenticationException("Access token is required.");
   }
}
