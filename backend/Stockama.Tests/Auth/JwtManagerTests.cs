using System.Linq.Expressions;
using Microsoft.AspNetCore.Http;
using Moq;
using Stockama.Core.Authorization;
using Stockama.Core.Authorization.Models;
using Stockama.Core.Data;
using Stockama.Core.Exeptions;
using Stockama.Data.Domain;
using Stockama.Helper;

namespace Stockama.Tests.Auth;

public class JwtManagerTests
{
   private readonly Mock<IRepository<User>> _repositoryMock;
   private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;

   public JwtManagerTests()
   {
      EnvironmentVariables.JwtTokenKey = "this-is-a-long-enough-jwt-test-key-123456";
      _repositoryMock = new Mock<IRepository<User>>();
      _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
   }

   [Fact]
   public async Task GenerateToken_ShouldThrowAuthenticationException_WhenUserNotFound()
   {
      var tokenUser = CreateTokenUser();
      _repositoryMock
         .Setup(r => r.Get(It.IsAny<Expression<Func<User, bool>>>()))
         .Returns((User)null!);

      var sut = CreateSut();

      var ex = await Assert.ThrowsAsync<AuthenticationException>(() => sut.GenerateToken(tokenUser));

      Assert.Equal("User not found", ex.Message);
   }

   [Fact]
   public async Task GenerateToken_ShouldReturnTokensAndPersistRefreshToken_WhenUserExists()
   {
      var tokenUser = CreateTokenUser();
      var user = CreateUser(tokenUser);
      SetupUserPersistence(user);

      var sut = CreateSut();
      var result = await sut.GenerateToken(tokenUser, "web");

      Assert.False(string.IsNullOrWhiteSpace(result.AccessToken));
      Assert.False(string.IsNullOrWhiteSpace(result.RefreshToken));
      Assert.Equal(result.RefreshToken, user.RefreshToken);
      Assert.True(result.ValidTo > DateTime.UtcNow.AddMinutes(50));

      _repositoryMock.Verify(
         r => r.UpdateBulkAsync(It.Is<List<User>>(list => list.Count == 1 && list[0] == user), null),
         Times.Once);
   }

   [Fact]
   public async Task RefreshAccessToken_ShouldReturnNewTokens_WhenRefreshBindingIsValid()
   {
      var tokenUser = CreateTokenUser();
      var user = CreateUser(tokenUser);
      SetupUserPersistence(user);

      var sut = CreateSut();
      var generated = await sut.GenerateToken(tokenUser, "web");
      var refreshed = await sut.RefreshAccessToken(generated.AccessToken, "web");

      Assert.False(string.IsNullOrWhiteSpace(refreshed.AccessToken));
      Assert.False(string.IsNullOrWhiteSpace(refreshed.RefreshToken));
      Assert.NotEqual(generated.RefreshToken, refreshed.RefreshToken);
      Assert.Equal(refreshed.RefreshToken, user.RefreshToken);
      Assert.True(user.RefreshTokenExpireDate > DateTime.UtcNow.AddDays(6));
   }

   [Fact]
   public async Task RefreshAccessToken_ShouldInvalidateUserTokens_WhenTokenBindingIsInvalid()
   {
      var tokenUser = CreateTokenUser();
      var user = CreateUser(tokenUser);
      SetupUserPersistence(user);

      var sut = CreateSut();
      var firstTokens = await sut.GenerateToken(tokenUser, "web");
      _ = await sut.GenerateToken(tokenUser, "admin");

      var ex = await Assert.ThrowsAsync<AuthenticationException>(() => sut.RefreshAccessToken(firstTokens.AccessToken, "web"));

      Assert.Equal("invalid token session", ex.Message);
      Assert.Null(user.RefreshToken);
      Assert.Null(user.RefreshTokenExpireDate);
   }

   [Fact]
   public async Task RevokeAccessToken_ShouldClearRefreshToken_WhenAccessTokenValid()
   {
      var tokenUser = CreateTokenUser();
      var user = CreateUser(tokenUser);
      SetupUserPersistence(user);

      var sut = CreateSut();
      var generated = await sut.GenerateToken(tokenUser, "web");

      var revoked = await sut.RevokeAccessToken(generated.AccessToken, "web");

      Assert.True(revoked);
      Assert.Null(user.RefreshToken);
      Assert.Null(user.RefreshTokenExpireDate);
   }

   [Fact]
   public async Task Validate_ShouldReturnFalse_WhenUserTokenStateIsRevoked()
   {
      var tokenUser = CreateTokenUser();
      var user = CreateUser(tokenUser);
      SetupUserPersistence(user);

      var sut = CreateSut();
      var generated = await sut.GenerateToken(tokenUser, "web");
      _ = await sut.RevokeAccessToken(generated.AccessToken, "web");

      var isValid = await sut.Validate(generated.AccessToken, "web");

      Assert.False(isValid);
   }

   [Fact]
   public async Task Validate_ShouldReturnFalse_ForInvalidToken()
   {
      var sut = CreateSut();

      var result = await sut.Validate("not-a-token");

      Assert.False(result);
   }

   private JwtManager CreateSut()
   {
      return new JwtManager(_httpContextAccessorMock.Object, _repositoryMock.Object);
   }

   private void SetupUserPersistence(User user)
   {
      _repositoryMock
         .Setup(r => r.Get(It.IsAny<Expression<Func<User, bool>>>()))
         .Returns(user);

      _repositoryMock
         .Setup(r => r.UpdateBulkAsync(It.IsAny<List<User>>(), null))
         .ReturnsAsync(true);
   }

   private static TokenUser CreateTokenUser()
   {
      return new TokenUser(
         Guid.Parse("11111111-1111-1111-1111-111111111111"),
         "john.doe",
         "john.doe@example.com",
         Guid.Parse("22222222-2222-2222-2222-222222222222"));
   }

   private static User CreateUser(TokenUser tokenUser)
   {
      return new User
      {
         Id = tokenUser.userId,
         Username = tokenUser.UserName,
         Email = tokenUser.Email,
         CompanyId = tokenUser.CompanyId,
         FirstName = "John",
         LastName = "Doe",
         PasswordHash = [],
         PasswordSalt = []
      };
   }
}
