using System.Linq.Expressions;
using Moq;
using Stockama.Application.Users.Query.GetUserListQuery;
using Stockama.Core.Data;
using Stockama.Core.Resources;
using Stockama.Data.Domain;

namespace Stockama.Tests.Users;

public class GetUserListQueryHandlerTests
{
   private readonly Mock<IResourceManager> _resourceManagerMock;
   private readonly Mock<IRepository<User>> _userRepositoryMock;
   private readonly Mock<IRepository<Company>> _companyRepositoryMock;

   public GetUserListQueryHandlerTests()
   {
      _resourceManagerMock = new Mock<IResourceManager>();
      _userRepositoryMock = new Mock<IRepository<User>>();
      _companyRepositoryMock = new Mock<IRepository<Company>>();
   }

   [Fact]
   public async Task HandleAsync_ShouldReturnForbidden_WhenRequesterIsNotSuperAdmin()
   {
      var requesterId = Guid.NewGuid();

      _userRepositoryMock
         .Setup(q => q.GetActiveAsync(It.IsAny<Expression<Func<User, bool>>>()))
         .ReturnsAsync(new User
         {
            Id = requesterId,
            IsSuperAdmin = false,
            Username = "u",
            Email = "u@x.com",
            PasswordHash = [],
            PasswordSalt = []
         });

      var sut = new GetUserListQueryHandler(_resourceManagerMock.Object, _userRepositoryMock.Object, _companyRepositoryMock.Object);

      var result = await sut.HandleAsync(new GetUserListQuery { UserId = requesterId });

      Assert.False(result.IsSuccess);
      Assert.Equal("403", result.Status);
   }

   [Fact]
   public async Task HandleAsync_ShouldReturnFilteredUsers_WhenRequesterIsSuperAdmin()
   {
      var requesterId = Guid.NewGuid();
      var companyId = Guid.NewGuid();

      _userRepositoryMock
         .SetupSequence(q => q.GetActiveAsync(It.IsAny<Expression<Func<User, bool>>>()))
         .ReturnsAsync(new User
         {
            Id = requesterId,
            IsSuperAdmin = true,
            Username = "sa",
            Email = "sa@x.com",
            PasswordHash = [],
            PasswordSalt = []
         });

      _userRepositoryMock
         .Setup(q => q.AllActiveAsync())
         .ReturnsAsync(
         [
            new User { Id = Guid.NewGuid(), CompanyId = companyId, IsTenantAdmin = true, FirstName = "A", LastName = "B", Username = "ab", Email = "ab@x.com", PasswordHash = [], PasswordSalt = [] },
            new User { Id = Guid.NewGuid(), CompanyId = Guid.NewGuid(), IsTenantAdmin = false, FirstName = "C", LastName = "D", Username = "cd", Email = "cd@x.com", PasswordHash = [], PasswordSalt = [] }
         ]);

      _companyRepositoryMock
         .Setup(q => q.AllActiveAsync())
         .ReturnsAsync([new Company { Id = companyId, Name = "ACME", CompanyCode = "acme", Description = "", LogoUrl = "", WebsiteUrl = "" }]);

      var sut = new GetUserListQueryHandler(_resourceManagerMock.Object, _userRepositoryMock.Object, _companyRepositoryMock.Object);

      var result = await sut.HandleAsync(new GetUserListQuery
      {
         UserId = requesterId,
         CompanyId = companyId,
         OnlyTenantAdmins = true
      });

      Assert.True(result.IsSuccess);
      Assert.Single(result.Data);
      Assert.Equal("ACME", result.Data.First().CompanyName);
   }
}
