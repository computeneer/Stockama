using System.Linq.Expressions;
using Moq;
using Stockama.Core.Authorization;
using Stockama.Core.Data;
using Stockama.Core.Exeptions;
using Stockama.Data.Domain;

namespace Stockama.Tests.Authorization;

public class UserPermissionManagerTests
{
   private readonly Mock<IRepository<UserPermission>> _permissionRepositoryMock;
   private readonly UserPermissionManager _sut;

   public UserPermissionManagerTests()
   {
      _permissionRepositoryMock = new Mock<IRepository<UserPermission>>();
      _sut = new UserPermissionManager(_permissionRepositoryMock.Object);
   }

   [Fact]
   public async Task AssignPermissionsAsync_ShouldCreateOnlyMissingPermissions()
   {
      var userId = Guid.NewGuid();
      var actorId = Guid.NewGuid();

      _permissionRepositoryMock
         .Setup(q => q.FilterActiveAsync(It.IsAny<Expression<Func<UserPermission, bool>>>()))
         .ReturnsAsync(
         [
            new UserPermission { UserId = userId, PermissionCode = "user.create" }
         ]);

      _permissionRepositoryMock
         .Setup(q => q.CreateBulkAsync(It.IsAny<List<UserPermission>>(), actorId))
         .ReturnsAsync(true);

      await _sut.AssignPermissionsAsync(
         userId,
         ["user.create", "user.update", " USER.UPDATE "],
         actorId);

      _permissionRepositoryMock.Verify(q =>
         q.CreateBulkAsync(
            It.Is<List<UserPermission>>(list =>
               list.Count == 1 &&
               list[0].UserId == userId &&
               list[0].PermissionCode == "user.update"),
            actorId),
         Times.Once);
   }

   [Fact]
   public async Task HasPermissionAsync_ShouldReturnRepositoryValue()
   {
      _permissionRepositoryMock
         .Setup(q => q.AnyActiveAsync(It.IsAny<Expression<Func<UserPermission, bool>>>()))
         .ReturnsAsync(true);

      var result = await _sut.HasPermissionAsync(Guid.NewGuid(), "stock.create");

      Assert.True(result);
   }

   [Fact]
   public async Task AssignPermissionsAsync_ShouldThrowValidation_WhenUserIdEmpty()
   {
      await Assert.ThrowsAsync<CustomValidationException>(() =>
         _sut.AssignPermissionsAsync(Guid.Empty, ["user.create"], Guid.NewGuid()));
   }
}
