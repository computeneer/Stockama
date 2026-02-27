using Stockama.Core.Middlewares;
using Microsoft.AspNetCore.Builder;

namespace Stockama.Utils.Extensions;

public static class ApplicationExtensions
{
   public static IApplicationBuilder UseGlobalErrorHandler(this IApplicationBuilder app)
   {
      app.UseMiddleware<GlobalErrorHandlerMiddleware>();
      return app;
   }

   public static IApplicationBuilder UseCustomAuthenticationMiddleware(this IApplicationBuilder app)
   {
      app.UseAuthentication();
      app.UseAuthorization();
      return app;
   }

   public static IApplicationBuilder UseAuthenticationMiddleware(this IApplicationBuilder app)
   {
      app.UseMiddleware<AuthenticationMiddleware>();
      return app;
   }

   public static IApplicationBuilder AddCommonServices(this IApplicationBuilder app)
   {
      app.UseCors("LocalPolicy");
      return app;
   }
}