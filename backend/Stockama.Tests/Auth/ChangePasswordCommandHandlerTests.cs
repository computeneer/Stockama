using System.Linq.Expressions;
using Moq;
using Stockama.Application.Auth.Commands.ChangePasswordCommand;
using Stockama.Core.Data;
using Stockama.Core.Resources;
using Stockama.Core.Security;
using Stockama.Data.Domain;

namespace Stockama.Tests.Auth;

public class ChangePasswordCommandHandlerTests
{
   private readonly Mock<IRepository<User>> _userRepositoryMock;
   private readonly Mock<IPasswordHasher> _passwordHasherMock;
   private readonly Mock<IResourceManager> _resourceManagerMock;

   public ChangePasswordCommandHandlerTests()
   {
      _userRepositoryMock = new Mock<IRepository<User>>();
      _passwordHasherMock = new Mock<IPasswordHasher>();
      _resourceManagerMock = new Mock<IResourceManager>();
   }

   [Fact]
   public async Task HandleAsync_ShouldUpdatePasswordAndClearMustChangeFlag_WhenCurrentPasswordValid()
   {
      var userId = Guid.NewGuid();
      var user = new User
      {
         Id = userId,
         Username = "tenant.admin",
         Email = "tenant@stockama.local",
         PasswordHash = [9],
         PasswordSalt = [8],
         MustChangePassword = true,
         OneTimePasswordUsed = true,
         FirstName = "Tenant",
         LastName = "Admin"
      };

      _userRepositoryMock
         .Setup(q => q.GetActiveAsync(It.IsAny<Expression<Func<User, bool>>>()))
         .ReturnsAsync(user);

      _passwordHasherMock
         .Setup(q => q.VerifyPassword("current", user.PasswordSalt, user.PasswordHash))
         .Returns(true);

      _passwordHasherMock
         .Setup(q => q.HashPassword("new-password"))
         .Returns((new byte[] { 1, 2 }, new byte[] { 3, 4 }));

      _userRepositoryMock
         .Setup(q => q.UpdateAsync(It.IsAny<User>(), userId))
         .ReturnsAsync(true);

      var sut = new ChangePasswordCommandHandler(_resourceManagerMock.Object, _userRepositoryMock.Object, _passwordHasherMock.Object);

      var result = await sut.HandleAsync(new ChangePasswordCommand
      {
         UserId = userId,
         CurrentPassword = "current",
         NewPassword = "new-password"
      });

      Assert.True(result.IsSuccess);
      Assert.False(user.MustChangePassword);
      Assert.Null(user.OneTimePasswordExpiresAt);

      _userRepositoryMock.Verify(q => q.UpdateAsync(It.Is<User>(u => u == user), userId), Times.Once);
   }

   [Fact]
   public async Task HandleAsync_ShouldReturnError_WhenCurrentPasswordInvalid()
   {
      var userId = Guid.NewGuid();
      var user = new User
      {
         Id = userId,
         Username = "tenant.admin",
         Email = "tenant@stockama.local",
         PasswordHash = [9],
         PasswordSalt = [8],
         FirstName = "Tenant",
         LastName = "Admin"
      };

      _userRepositoryMock
         .Setup(q => q.GetActiveAsync(It.IsAny<Expression<Func<User, bool>>>()))
         .ReturnsAsync(user);

      _passwordHasherMock
         .Setup(q => q.VerifyPassword("wrong", user.PasswordSalt, user.PasswordHash))
         .Returns(false);

      var sut = new ChangePasswordCommandHandler(_resourceManagerMock.Object, _userRepositoryMock.Object, _passwordHasherMock.Object);

      var result = await sut.HandleAsync(new ChangePasswordCommand
      {
         UserId = userId,
         CurrentPassword = "wrong",
         NewPassword = "new-password"
      });

      Assert.False(result.IsSuccess);
      _userRepositoryMock.Verify(q => q.UpdateAsync(It.IsAny<User>(), It.IsAny<Guid?>()), Times.Never);
   }
}
