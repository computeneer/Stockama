using System.Linq.Expressions;
using Moq;
using Stockama.Application.Auth.Commands.LoginCommand;
using Stockama.Core.Authorization;
using Stockama.Core.Authorization.Models;
using Stockama.Core.Cache;
using Stockama.Core.Cache.CacheModels;
using Stockama.Core.Data;
using Stockama.Core.Resources;
using Stockama.Core.Security;
using Stockama.Data.Domain;

namespace Stockama.Tests.Auth;

public class LoginCommandHandlerTests
{
   private readonly Mock<IRepository<User>> _userRepositoryMock;
   private readonly Mock<IPasswordHasher> _passwordHasherMock;
   private readonly Mock<IJwtManager> _jwtManagerMock;
   private readonly Mock<IResourceManager> _resourceManagerMock;
   private readonly Mock<ICacheManager> _cacheManagerMock;

   public LoginCommandHandlerTests()
   {
      _userRepositoryMock = new Mock<IRepository<User>>();
      _passwordHasherMock = new Mock<IPasswordHasher>();
      _jwtManagerMock = new Mock<IJwtManager>();
      _resourceManagerMock = new Mock<IResourceManager>();
      _cacheManagerMock = new Mock<ICacheManager>();
   }

   [Fact]
   public async Task HandleAsync_ShouldMarkOneTimePasswordUsed_AndRequirePasswordChange_WhenFirstLoginWithOtp()
   {
      var user = CreateTenantAdminUser();
      user.MustChangePassword = true;
      user.OneTimePasswordUsed = false;
      user.OneTimePasswordExpiresAt = DateTimeOffset.UtcNow.AddMinutes(10);

      _userRepositoryMock
         .Setup(q => q.GetActiveAsync(It.IsAny<Expression<Func<User, bool>>>()))
         .ReturnsAsync(user);

      _passwordHasherMock
         .Setup(q => q.VerifyPassword("otp-password", user.PasswordSalt, user.PasswordHash))
         .Returns(true);

      _userRepositoryMock
         .Setup(q => q.UpdateAsync(user, user.Id))
         .ReturnsAsync(true);

      _cacheManagerMock
         .Setup(q => q.GetCompanyCacheList())
         .ReturnsAsync([new CompanyCacheModel { Id = user.CompanyId, CompanyCode = "acme" }]);

      _jwtManagerMock
         .Setup(q => q.GenerateToken(It.IsAny<TokenUser>(), It.IsAny<string>()))
         .ReturnsAsync(new JwtTokens("access", "refresh", DateTime.UtcNow.AddHours(1)));

      var sut = new LoginCommandHandler(_resourceManagerMock.Object, _userRepositoryMock.Object, _passwordHasherMock.Object, _jwtManagerMock.Object, _cacheManagerMock.Object);

      var result = await sut.HandleAsync(new LoginCommand
      {
         Username = user.Username,
         Password = "otp-password",
         CompanyCode = "acme"
      });

      Assert.True(result.IsSuccess);
      Assert.True(result.Data.RequirePasswordChange);
      Assert.True(user.OneTimePasswordUsed);
      _userRepositoryMock.Verify(q => q.UpdateAsync(user, user.Id), Times.Once);
   }

   [Fact]
   public async Task HandleAsync_ShouldReturnError_WhenOneTimePasswordAlreadyUsed()
   {
      var user = CreateTenantAdminUser();
      user.MustChangePassword = true;
      user.OneTimePasswordUsed = true;
      user.OneTimePasswordExpiresAt = DateTimeOffset.UtcNow.AddMinutes(10);

      _userRepositoryMock
         .Setup(q => q.GetActiveAsync(It.IsAny<Expression<Func<User, bool>>>()))
         .ReturnsAsync(user);

      _passwordHasherMock
         .Setup(q => q.VerifyPassword("otp-password", user.PasswordSalt, user.PasswordHash))
         .Returns(true);

      _cacheManagerMock
         .Setup(q => q.GetCompanyCacheList())
         .ReturnsAsync([new CompanyCacheModel { Id = user.CompanyId, CompanyCode = "acme" }]);

      var sut = new LoginCommandHandler(_resourceManagerMock.Object, _userRepositoryMock.Object, _passwordHasherMock.Object, _jwtManagerMock.Object, _cacheManagerMock.Object);

      var result = await sut.HandleAsync(new LoginCommand
      {
         Username = user.Username,
         Password = "otp-password",
         CompanyCode = "acme"
      });

      Assert.False(result.IsSuccess);
      _jwtManagerMock.Verify(q => q.GenerateToken(It.IsAny<TokenUser>(), It.IsAny<string>()), Times.Never);
   }

   private static User CreateTenantAdminUser()
   {
      return new User
      {
         Id = Guid.NewGuid(),
         Username = "tenant.admin",
         Email = "tenant@stockama.local",
         PasswordHash = [1, 2],
         PasswordSalt = [3, 4],
         CompanyId = Guid.NewGuid(),
         FirstName = "Admin",
         LastName = "Admin"
      };
   }
}
