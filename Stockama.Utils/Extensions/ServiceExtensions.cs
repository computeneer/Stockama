using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

using Stockama.Data;
using Stockama.Core.Auth;
using Stockama.Helper;
using Stockama.Core.Data;
using Stockama.Core.Security;
using Stockama.Core.Resources;
using Stockama.Core.Cache;

using LiteBus.Extensions.Microsoft.DependencyInjection;
using LiteBus.Commands;
using LiteBus.Queries;
using LiteBus.Events;

namespace Stockama.Utils.Extensions;

public static class ServiceExtensions
{
   public static IServiceCollection AddEnvironmentVariables(this IServiceCollection services)
   {
      var root = Directory.GetCurrentDirectory();
      var parent = Directory.GetParent(root);
      var filePath = Path.Combine(parent!.FullName, ".env");
      {
         if (File.Exists(filePath))
         {
            foreach (var line in File.ReadAllLines(filePath))
            {
               var parts = line.Split(
                   '=',
                   StringSplitOptions.RemoveEmptyEntries);

               if (parts.Length != 2)
                  continue;

               Environment.SetEnvironmentVariable(parts[0], parts[1]);
            }
         }
      }

      return services;
   }

   public static IServiceCollection AddCommonServices(this IServiceCollection services)
   {
      services.AddHttpContextAccessor();
      services.AddOpenApi();

      services.AddLiteBus(liteBus =>
         {
            var appAssembly = typeof(Application.Auth.Commands.LoginCommand.LoginCommand).Assembly;

            liteBus.AddCommandModule(module => module.RegisterFromAssembly(appAssembly));
            liteBus.AddQueryModule(module => module.RegisterFromAssembly(appAssembly));
            liteBus.AddEventModule(module => module.RegisterFromAssembly(appAssembly));
         });

      services.AddCors(options =>
      {
         options.AddPolicy("LocalPolicy", policy =>
         {
            policy
               .WithOrigins("http://localhost:5173", "https://localhost:5173", "http://127.0.0.1:5173", "https://[::1]:5173")
               .AllowAnyHeader()
               .AllowAnyMethod()
               .AllowCredentials();
         });
      });

      services.AddAuthentication(opt =>
         {
            opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
         }).AddJwtBearer(opt =>
            {
               opt.RequireHttpsMetadata = false;
               opt.SaveToken = true;
               opt.TokenValidationParameters = new TokenValidationParameters
               {
                  ValidateIssuer = true,
                  ValidateAudience = false,
                  ValidateLifetime = true,
                  IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(EnvironmentVariables.JwtTokenKey)),
                  ClockSkew = TimeSpan.Zero
               };
            });

      services.AddAuthorization();

      services.AddDbContext<DbContext, DataContext>();

      services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
      services.AddScoped<IJwtManager, JwtManager>();
      services.AddScoped<IPasswordHasher, PasswordHasher>();
      services.AddScoped<IResourceManager, ResourceManager>();
      services.AddScoped<ICacheUnit, RedisCacheUnit>();
      services.AddScoped<ICacheManager, CacheManager>();

      return services;
   }
}
