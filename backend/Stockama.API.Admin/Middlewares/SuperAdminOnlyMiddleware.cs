using Stockama.Core.Authorization;
using Stockama.Core.Model.Response;
using Stockama.Helper.Constants;

namespace Stockama.API.Admin.Middlewares;

public class SuperAdminOnlyMiddleware
{
   private readonly RequestDelegate _next;

   public SuperAdminOnlyMiddleware(RequestDelegate next)
   {
      _next = next;
   }

   public async Task InvokeAsync(HttpContext context, IJwtManager jwtManager)
   {
      var allowedUrls = new[]
      {
         "/api/auth/login",
         "/api/auth/refresh",
         "/api/auth/validate",
         "/api/auth/logout",
         "/api/auth/revoke",
      };

      var path = context.Request.Path.Value?.ToLowerInvariant() ?? string.Empty;
      if (path.StartsWith("/openapi") || allowedUrls.Contains(path))
      {
         await _next(context);
         return;
      }

      if (context.User?.Identity?.IsAuthenticated != true)
      {
         context.Response.StatusCode = StatusCodes.Status401Unauthorized;
         await context.Response.WriteAsJsonAsync(new ErrorBoolResponse("401", "Unauthorized"));
         return;
      }

      var authHeader = context.Request.Headers.Authorization.ToString();

      if (string.IsNullOrEmpty(authHeader))
      {
         context.Response.StatusCode = StatusCodes.Status403Forbidden;
         await context.Response.WriteAsJsonAsync(new ErrorBoolResponse("403", "Token not found."));
         return;
      }

      var tokenValidationResult = await jwtManager.Validate(authHeader);
      if (!tokenValidationResult)
      {
         context.Response.StatusCode = StatusCodes.Status403Forbidden;
         await context.Response.WriteAsJsonAsync(new ErrorBoolResponse("403", "Invalid token."));
         return;
      }

      var isSuperAdminClaim = context.User.Claims.FirstOrDefault(q => q.Type == "IsSuperAdmin")?.Value;
      var isSuperAdmin = bool.TryParse(isSuperAdminClaim, out var parsed) && parsed;

      if (!isSuperAdmin)
      {
         context.Response.StatusCode = StatusCodes.Status403Forbidden;
         await context.Response.WriteAsJsonAsync(new ErrorBoolResponse("403", "Only super admin can access this endpoint."));
         return;
      }

      context.Items[MiddlewareConstants.SuperAdminBypassAuthenticationMiddlewareKey] = true;

      await _next(context);
   }
}
