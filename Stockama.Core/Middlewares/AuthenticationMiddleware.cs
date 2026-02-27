using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.JsonWebTokens;
using Stockama.Core.Auth;

namespace Stockama.Core.Middlewares;

public class AuthenticationMiddleware
{
   private readonly RequestDelegate _next;
   private readonly JsonWebTokenHandler _tokenHandler;
   private readonly IJwtManager _jwtManager;

   public AuthenticationMiddleware(RequestDelegate next, IJwtManager jwtManager)
   {
      _next = next;
      _tokenHandler = new JsonWebTokenHandler();
      _jwtManager = jwtManager;
      _jwtManager = new JwtManager();
   }

   // ?????????? USER TABLOSUNA BAKILMALI MI? BAKILMAMALI MI? ???????????///
   // ?????????? Her REQUESTTE DB'ye gitmek mantikli mi? Performans mi daha onemli, guvenlik mi? ?????????????
   // ?????????? Simdilik sadece tokeni ciddiye alicam. DB'ye gitmiyoruz. ?????????????
   // TODO: Ileride her request icin db sorgusu dusunulebilir... ?????????????
   public async Task Invoke(HttpContext context)
   {
      var isPermitted = false;
      // Guid userId;

      var allowedUrls = new[] { "/api/auth/login", "/api/auth/register", "/api/test/healthcheck" };

      var pathValue = context.Request.Path.Value?.ToLower();

      if (allowedUrls.Contains(pathValue))
      {
         isPermitted = true;
      }
      else
      {
         try
         {
            string? authHeader = context.Request.Headers["Authorization"];

            if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase) && authHeader != "Bearer null")
            {
               var token = _tokenHandler.ReadJsonWebToken(authHeader);

               if (token == null)
               {
                  // TODO: CUSTOM EXCEPTION
                  throw new Exception("Token Okunamadi.");
               }

               var tokenValidationResult = await _jwtManager.Validate(authHeader);

               isPermitted = tokenValidationResult;
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
         // TODO: Return CUSTOM BASERETURN MODEL
         context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
         await context.Response.WriteAsync("Unauthorized");
      }
   }

}