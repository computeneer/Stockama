using LiteBus.Commands.Abstractions;
using Microsoft.AspNetCore.Http;
using Stockama.Application.Auth.Commands;
using Stockama.Core.Authorization;
using Stockama.Core.Base;
using Stockama.Core.Model.Response;
using Stockama.Core.Resources;

namespace Stockama.Application.Auth.Commands.RevokeAccessTokenCommand;

public sealed class RevokeAccessTokenCommandHandler : BaseHandler, ICommandHandler<RevokeAccessTokenCommand, IBaseBoolResponse>
{
   private readonly IJwtManager _jwtManager;
   private readonly IHttpContextAccessor _httpContextAccessor;

   public RevokeAccessTokenCommandHandler(
      IResourceManager resourceManager,
      IJwtManager jwtManager,
      IHttpContextAccessor httpContextAccessor) : base(resourceManager)
   {
      _jwtManager = jwtManager;
      _httpContextAccessor = httpContextAccessor;
   }

   public async Task<IBaseBoolResponse> HandleAsync(RevokeAccessTokenCommand message, CancellationToken cancellationToken = default)
   {
      var accessToken = AccessTokenResolver.ResolveFromAuthorizationHeader(_httpContextAccessor);
      var revoked = await _jwtManager.RevokeAccessToken(accessToken, message.ClientType);
      return new SuccessBoolResponse(revoked);
   }
}
