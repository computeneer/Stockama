using System.Linq.Expressions;
using Moq;
using Stockama.Core.Authorization;
using Stockama.Core.Data;
using Stockama.Core.Exeptions;
using Stockama.Core.Queue;
using Stockama.Core.Queue.Models;
using Stockama.Core.Security;
using Stockama.Core.Tenants;
using Stockama.Core.Tenants.Models;
using Stockama.Data.Domain;
using Stockama.Helper.Constants;

namespace Stockama.Tests.Tenants;

public class TenantProvisionManagerTests
{
   private readonly Mock<IRepository<Company>> _companyRepositoryMock;
   private readonly Mock<IRepository<User>> _userRepositoryMock;
   private readonly Mock<IUserPermissionManager> _userPermissionManagerMock;
   private readonly Mock<IPasswordHasher> _passwordHasherMock;
   private readonly Mock<IQueueManager> _queueManagerMock;

   private readonly TenantProvisionManager _sut;

   public TenantProvisionManagerTests()
   {
      _companyRepositoryMock = new Mock<IRepository<Company>>();
      _userRepositoryMock = new Mock<IRepository<User>>();
      _userPermissionManagerMock = new Mock<IUserPermissionManager>();
      _passwordHasherMock = new Mock<IPasswordHasher>();
      _queueManagerMock = new Mock<IQueueManager>();

      _sut = new TenantProvisionManager(
         _companyRepositoryMock.Object,
         _userRepositoryMock.Object,
         _userPermissionManagerMock.Object,
         _passwordHasherMock.Object,
         _queueManagerMock.Object);
   }

   [Fact]
   public async Task ProvisionTenantAsync_ShouldThrowUnauthorized_WhenRequesterIsNotSuperAdmin()
   {
      var request = CreateRequest();

      _userRepositoryMock
         .Setup(q => q.GetActiveAsync(It.IsAny<Expression<Func<User, bool>>>()))
         .ReturnsAsync(new User { Id = request.SuperAdminUserId, IsSuperAdmin = false });

      await Assert.ThrowsAsync<HttpUnauthorizedException>(() => _sut.ProvisionTenantAsync(request));
   }

   [Fact]
   public async Task ProvisionTenantAsync_ShouldCreateTenantAndQueueOneTimePassword_WhenRequestValid()
   {
      var request = CreateRequest();
      User createdTenantAdmin = null;
      QueueTemplateMessageRequest queueRequest = null;

      _userRepositoryMock
         .Setup(q => q.GetActiveAsync(It.IsAny<Expression<Func<User, bool>>>()))
         .ReturnsAsync(new User
         {
            Id = request.SuperAdminUserId,
            IsSuperAdmin = true,
            Username = "superadmin",
            Email = "superadmin@stockama.local",
            PasswordHash = [],
            PasswordSalt = []
         });

      _companyRepositoryMock
         .SetupSequence(q => q.AnyActiveAsync(It.IsAny<Expression<Func<Company, bool>>>()))
         .ReturnsAsync(false)
         .ReturnsAsync(false);

      _userRepositoryMock
         .Setup(q => q.AnyActiveAsync(It.IsAny<Expression<Func<User, bool>>>()))
         .ReturnsAsync(false);

      _passwordHasherMock
         .Setup(q => q.HashPassword(It.IsAny<string>()))
         .Returns((new byte[] { 1, 2 }, new byte[] { 3, 4 }));

      _companyRepositoryMock
         .Setup(q => q.CreateBulkAsync(It.IsAny<List<Company>>(), request.SuperAdminUserId))
         .Callback<List<Company>, Guid>((companies, _) =>
         {
            foreach (var company in companies.Where(company => company.Id == Guid.Empty))
            {
               company.Id = Guid.NewGuid();
            }
         })
         .ReturnsAsync(true);

      _userRepositoryMock
         .Setup(q => q.CreateBulkAsync(It.IsAny<List<User>>(), request.SuperAdminUserId))
         .Callback<List<User>, Guid>((users, _) =>
         {
            createdTenantAdmin = users.Single();
            if (createdTenantAdmin.Id == Guid.Empty)
            {
               createdTenantAdmin.Id = Guid.NewGuid();
            }
         })
         .ReturnsAsync(true);

      _queueManagerMock
         .Setup(q => q.EnqueueTemplateMessageAsync(It.IsAny<QueueTemplateMessageRequest>(), It.IsAny<CancellationToken>()))
         .Callback<QueueTemplateMessageRequest, CancellationToken>((requestModel, _) => queueRequest = requestModel)
         .Returns(Task.CompletedTask);

      var result = await _sut.ProvisionTenantAsync(request);

      Assert.NotEqual(Guid.Empty, result.CompanyId);
      Assert.NotEqual(Guid.Empty, result.TenantAdminUserId);
      Assert.Equal(result.TenantAdminUserId, createdTenantAdmin.Id);
      Assert.True(createdTenantAdmin.MustChangePassword);
      Assert.True(createdTenantAdmin.IsTenantAdmin);
      Assert.False(createdTenantAdmin.IsSuperAdmin);
      Assert.Equal(QueueConstants.OneTimePasswordNotificationQueueName, queueRequest.QueueName);
      Assert.Equal(MessageTemplateConstants.TenantAdminOneTimePasswordTemplateKey, queueRequest.TemplateKey);
      Assert.Equal(createdTenantAdmin.LanguageId, queueRequest.LanguageId);
      Assert.True(queueRequest.TemplateValues.ContainsKey("OneTimePassword"));
      Assert.Equal(ApplicationContants.OTP_CODE_LENGTH, queueRequest.TemplateValues["OneTimePassword"].Length);

      _userPermissionManagerMock.Verify(q =>
         q.AssignPermissionsAsync(createdTenantAdmin.Id, PermissionConstants.TenantAdminPermissions, request.SuperAdminUserId),
         Times.Once);
   }

   private static TenantProvisionRequest CreateRequest()
   {
      return new TenantProvisionRequest
      {
         SuperAdminUserId = Guid.NewGuid(),
         CompanyName = "ACME",
         Description = "Tenant",
         CompanyCode = "acme",
         LogoUrl = string.Empty,
         WebsiteUrl = string.Empty,
         AdminFirstName = "Tenant",
         AdminLastName = "Admin",
         AdminEmail = "admin@acme.local"
      };
   }
}
