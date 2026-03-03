using System.Linq.Expressions;
using System.Security.Claims;
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
         .Returns((User)null);

      var sut = CreateSut();

      var ex = await Assert.ThrowsAsync<AuthenticationException>(() => sut.GenerateToken(tokenUser));

      Assert.Equal("User not found", ex.Message);
   }

   [Fact]
   public async Task GenerateToken_ShouldReturnTokensAndPersistRefreshToken_WhenUserExists()
   {
      var tokenUser = CreateTokenUser();
      var user = CreateUser(tokenUser);

      _repositoryMock
         .Setup(r => r.Get(It.IsAny<Expression<Func<User, bool>>>()))
         .Returns(user);

      _repositoryMock
         .Setup(r => r.UpdateBulkAsync(It.IsAny<List<User>>(), null))
         .ReturnsAsync(true);

      var sut = CreateSut();

      var result = await sut.GenerateToken(tokenUser);

      Assert.False(string.IsNullOrWhiteSpace(result.AccessToken));
      Assert.False(string.IsNullOrWhiteSpace(result.RefreshToken));
      Assert.Equal(result.RefreshToken, user.RefreshToken);
      Assert.True(result.ValidTo > DateTime.UtcNow.AddMinutes(50));
      Assert.True(result.ValidTo <= DateTime.UtcNow.AddHours(1).AddMinutes(1));

      _repositoryMock.Verify(
         r => r.UpdateBulkAsync(
            It.Is<List<User>>(list => list.Count == 1 && list[0] == user),
            null),
         Times.Once);
   }

   [Fact]
   public async Task RefreshToken_ShouldThrowException_WhenHttpContextIsMissing()
   {
      _httpContextAccessorMock.SetupGet(x => x.HttpContext).Returns((HttpContext)null);
      var sut = CreateSut();

      var ex = await Assert.ThrowsAsync<Exception>(() => sut.RefreshToken("any-token"));

      Assert.Equal("User is empty", ex.Message);
   }

   [Fact]
   public async Task RefreshToken_ShouldThrowException_WhenRefreshTokenDoesNotMatch()
   {
      var tokenUser = CreateTokenUser();
      var user = CreateUser(tokenUser);
      user.RefreshToken = "stored-token";
      user.RefreshTokenExpireDate = DateTime.UtcNow.AddDays(1);

      _httpContextAccessorMock.SetupGet(x => x.HttpContext).Returns(CreateHttpContext(tokenUser.userId));
      _repositoryMock.Setup(r => r.Get(It.IsAny<Expression<Func<User, bool>>>())).Returns(user);

      var sut = CreateSut();

      var ex = await Assert.ThrowsAsync<Exception>(() => sut.RefreshToken("different-token"));

      Assert.Equal("invalid refresh token", ex.Message);
   }

   [Fact]
   public async Task RefreshToken_ShouldThrowException_WhenRefreshTokenExpired()
   {
      var tokenUser = CreateTokenUser();
      var user = CreateUser(tokenUser);
      user.RefreshToken = "stored-token";
      user.RefreshTokenExpireDate = DateTime.UtcNow.AddSeconds(-1);

      _httpContextAccessorMock.SetupGet(x => x.HttpContext).Returns(CreateHttpContext(tokenUser.userId));
      _repositoryMock.Setup(r => r.Get(It.IsAny<Expression<Func<User, bool>>>())).Returns(user);

      var sut = CreateSut();

      var ex = await Assert.ThrowsAsync<Exception>(() => sut.RefreshToken("stored-token"));

      Assert.Equal("refresh token expired", ex.Message);
   }

   [Fact]
   public async Task RefreshToken_ShouldReturnNewTokensAndUpdateUser_WhenRefreshTokenValid()
   {
      var tokenUser = CreateTokenUser();
      var user = CreateUser(tokenUser);
      user.RefreshToken = "stored-token";
      user.RefreshTokenExpireDate = DateTime.UtcNow.AddDays(1);

      _httpContextAccessorMock.SetupGet(x => x.HttpContext).Returns(CreateHttpContext(tokenUser.userId));
      _repositoryMock.Setup(r => r.Get(It.IsAny<Expression<Func<User, bool>>>())).Returns(user);
      _repositoryMock.Setup(r => r.Update(It.IsAny<User>(), null)).Returns(true);

      var sut = CreateSut();

      var result = await sut.RefreshToken("stored-token");

      Assert.False(string.IsNullOrWhiteSpace(result.AccessToken));
      Assert.False(string.IsNullOrWhiteSpace(result.RefreshToken));
      Assert.NotEqual("stored-token", result.RefreshToken);
      Assert.Equal(result.RefreshToken, user.RefreshToken);
      Assert.True(user.RefreshTokenExpireDate > DateTime.UtcNow.AddDays(6));

      _repositoryMock.Verify(r => r.Update(It.Is<User>(u => u == user), null), Times.Once);
   }

   [Fact]
   public async Task Validate_ShouldReturnTrue_ForGeneratedAccessToken()
   {
      var tokenUser = CreateTokenUser();
      var user = CreateUser(tokenUser);

      _repositoryMock.Setup(r => r.Get(It.IsAny<Expression<Func<User, bool>>>())).Returns(user);
      _repositoryMock.Setup(r => r.UpdateBulkAsync(It.IsAny<List<User>>(), null)).ReturnsAsync(true);

      var sut = CreateSut();
      var tokens = await sut.GenerateToken(tokenUser);

      var isValid = await sut.Validate(tokens.AccessToken);
      var isValidFromModel = await sut.Validate(tokens);

      Assert.True(isValid);
      Assert.True(isValidFromModel);
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

   private static HttpContext CreateHttpContext(Guid userId)
   {
      var context = new DefaultHttpContext();
      var claims = new List<Claim> { new(ClaimTypes.NameIdentifier, userId.ToString()) };
      context.User = new ClaimsPrincipal(new ClaimsIdentity(claims, "TestAuth"));
      return context;
   }
}
