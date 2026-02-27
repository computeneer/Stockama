using System.Net;
using System.Text.Json;
using Stockama.Core.Exeptions;
using Stockama.Core.Model.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;

namespace Stockama.Core.Middlewares;


public class GlobalErrorHandlerMiddleware
{

   private readonly RequestDelegate _next;
   private readonly JsonSerializerOptions _jsonOptions;

   public GlobalErrorHandlerMiddleware(RequestDelegate next, IOptions<JsonOptions> jsonOptions)
   {
      _next = next;
      _jsonOptions = jsonOptions.Value.SerializerOptions;
   }

   public async Task Invoke(HttpContext context)
   {
      try
      {
         await _next(context);
      }
      catch (Exception exception)
      {
         var response = context.Response;
         response.ContentType = "application/json";

         var statusCode = exception switch
         {
            HttpUnauthorizedException => HttpStatusCode.Unauthorized,
            HttpBadRequestException => HttpStatusCode.BadRequest,
            HttpNotFoundExeption => HttpStatusCode.NotFound,
            HttpConfictExeptions => HttpStatusCode.Conflict,
            AuthenticationException => HttpStatusCode.Unauthorized,
            System.Security.Authentication.AuthenticationException => HttpStatusCode.Unauthorized,
            CustomValidationException => HttpStatusCode.BadRequest,
            _ => HttpStatusCode.BadRequest
         };

         response.StatusCode = 200;

         var responseModel = JsonSerializer.Serialize(new BaseErrorResponse<string>(statusCode, exception.Message), _jsonOptions);
         await response.WriteAsync(responseModel);

      }
   }
}