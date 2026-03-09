using LiteBus.Commands.Abstractions;
using Microsoft.AspNetCore.Http;
using Stockama.Application.Auth.Commands;
using Stockama.Application.Auth.Models;
using Stockama.Core.Authorization;
using Stockama.Core.Base;
using Stockama.Core.Model.Response;
using Stockama.Core.Resources;

namespace Stockama.Application.Auth.Commands.ValidateAccessTokenCommand;

public sealed class ValidateAccessTokenCommandHandler : BaseHandler, ICommandHandler<ValidateAccessTokenCommand, IBaseDataResponse<ValidateTokenResponse>>
{
   private readonly IJwtManager _jwtManager;
   private readonly IHttpContextAccessor _httpContextAccessor;

   public ValidateAccessTokenCommandHandler(
      IResourceManager resourceManager,
      IJwtManager jwtManager,
      IHttpContextAccessor httpContextAccessor) : base(resourceManager)
   {
      _jwtManager = jwtManager;
      _httpContextAccessor = httpContextAccessor;
   }

   public async Task<IBaseDataResponse<ValidateTokenResponse>> HandleAsync(ValidateAccessTokenCommand message, CancellationToken cancellationToken = default)
   {
      var accessToken = AccessTokenResolver.ResolveFromAuthorizationHeader(_httpContextAccessor);
      var isValid = await _jwtManager.Validate(accessToken, message.ClientType);
      return new SuccessDataResponse<ValidateTokenResponse>(new ValidateTokenResponse(isValid));
   }
}
