using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.JsonWebTokens;
using Stockama.Core.Authorization;
using Stockama.Core.Exeptions;
using Stockama.Core.Model.Response;
using Stockama.Helper.Constants;
using Stockama.Helper;

namespace Stockama.Core.Middlewares;

public class AuthenticationMiddleware
{
   private readonly RequestDelegate _next;
   private readonly JsonWebTokenHandler _tokenHandler;

   public AuthenticationMiddleware(RequestDelegate next)
   {
      _next = next;
      _tokenHandler = new JsonWebTokenHandler();
   }

   // ?????????? USER TABLOSUNA BAKILMALI MI? BAKILMAMALI MI? ???????????///
   // ?????????? Her REQUESTTE DB'ye gitmek mantikli mi? Performans mi daha onemli, guvenlik mi? ?????????????
   // ?????????? Simdilik sadece tokeni ciddiye alicam. DB'ye gitmiyoruz. ?????????????
   // TODO: Ileride her request icin db sorgusu dusunulebilir... ?????????????
   public async Task InvokeAsync(HttpContext context, IJwtManager _jwtManager)
   {
      if (context.Items.TryGetValue(MiddlewareConstants.SuperAdminBypassAuthenticationMiddlewareKey, out var bypassValue)
         && bypassValue is bool bypass
         && bypass)
      {
         await _next(context);
         return;
      }

      var isPermitted = false;
      var expectedClientType = EnvironmentVariables.AuthClientType;
      var allowedUrls = new[]
      {
         "/api/auth/login",
         "/api/auth/refresh",
         "/api/auth/validate",
         "/api/auth/logout",
         "/api/auth/revoke",
      };

      var pathValue = context.Request.Path.Value?.ToLower();

      if (allowedUrls.Contains(pathValue))
      {
         isPermitted = true;
      }
      else
      {
         try
         {
            var authHeader = context.Request.Headers.Authorization.ToString();

            if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase) && authHeader != "Bearer null")
            {
               var rawToken = authHeader["Bearer ".Length..].Trim();
               if (string.IsNullOrWhiteSpace(rawToken))
               {
                  throw new AuthenticationException("Token Okunamadi.");
               }

               var tokenValidationResult = await _jwtManager.Validate(rawToken, expectedClientType);
               if (tokenValidationResult)
               {
                  isPermitted = true;
               }
               else
               {
                  var refreshedAccessToken = await TryRefreshExpiredAccessTokenAsync(rawToken, expectedClientType, _jwtManager);
                  if (!string.IsNullOrWhiteSpace(refreshedAccessToken))
                  {
                     context.Request.Headers.Authorization = $"Bearer {refreshedAccessToken}";
                     context.Response.Headers[MiddlewareConstants.RefreshedAccessTokenHeaderName] = refreshedAccessToken;
                     context.User = CreatePrincipalFromToken(refreshedAccessToken);
                     isPermitted = true;
                  }
                  else
                  {
                     isPermitted = false;
                  }
               }
            }
            else
            {
               isPermitted = false;
            }
         }
         catch (Exception ex)
         {
            // TODO: LOGGER KULLANABILIRSIN YA DA LOGLAMADAN VAZGECEBILIRSIN. SANA KALMIS..
            Console.WriteLine(ex.ToString());
            Console.WriteLine(ex.InnerException?.ToString());
            isPermitted = false;
         }
      }
      if (isPermitted)
      {
         await _next(context);
      }
      else
      {
         context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
         await context.Response.WriteAsJsonAsync(new ErrorBoolResponse("401"));
      }
   }

   private async Task<string?> TryRefreshExpiredAccessTokenAsync(string rawToken, string expectedClientType, IJwtManager jwtManager)
   {
      try
      {
         var token = _tokenHandler.ReadJsonWebToken(rawToken);
         if (token == null || token.ValidTo > DateTime.UtcNow)
         {
            return null;
         }

         var refreshedTokens = await jwtManager.RefreshAccessToken(rawToken, expectedClientType);
         return refreshedTokens.AccessToken;
      }
      catch
      {
         return null;
      }
   }

   private ClaimsPrincipal CreatePrincipalFromToken(string accessToken)
   {
      var token = _tokenHandler.ReadJsonWebToken(accessToken);
      var identity = new ClaimsIdentity(token.Claims, "Bearer", ClaimTypes.NameIdentifier, ClaimTypes.Role);
      return new ClaimsPrincipal(identity);
   }

}
