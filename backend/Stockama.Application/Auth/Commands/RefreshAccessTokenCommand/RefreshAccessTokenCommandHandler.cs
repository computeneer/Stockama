using LiteBus.Commands.Abstractions;
using Microsoft.AspNetCore.Http;
using Stockama.Application.Auth.Commands;
using Stockama.Application.Auth.Models;
using Stockama.Core.Authorization;
using Stockama.Core.Base;
using Stockama.Core.Model.Response;
using Stockama.Core.Resources;

namespace Stockama.Application.Auth.Commands.RefreshAccessTokenCommand;

public sealed class RefreshAccessTokenCommandHandler : BaseHandler, ICommandHandler<RefreshAccessTokenCommand, IBaseDataResponse<LoginResponse>>
{
   private readonly IJwtManager _jwtManager;
   private readonly IHttpContextAccessor _httpContextAccessor;

   public RefreshAccessTokenCommandHandler(
      IResourceManager resourceManager,
      IJwtManager jwtManager,
      IHttpContextAccessor httpContextAccessor) : base(resourceManager)
   {
      _jwtManager = jwtManager;
      _httpContextAccessor = httpContextAccessor;
   }

   public async Task<IBaseDataResponse<LoginResponse>> HandleAsync(RefreshAccessTokenCommand message, CancellationToken cancellationToken = default)
   {
      var accessToken = AccessTokenResolver.ResolveFromAuthorizationHeader(_httpContextAccessor);
      var tokens = await _jwtManager.RefreshAccessToken(accessToken, message.ClientType);
      var response = new LoginResponse(tokens.AccessToken, tokens.ValidTo, false);
      return new SuccessDataResponse<LoginResponse>(response);
   }
}
