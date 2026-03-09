using System.Linq.Expressions;
using Moq;
using Stockama.Application.Users.Commands.ResetUserOneTimePasswordCommand;
using Stockama.Core.Data;
using Stockama.Core.Queue;
using Stockama.Core.Queue.Models;
using Stockama.Core.Resources;
using Stockama.Core.Security;
using Stockama.Data.Domain;
using Stockama.Helper.Constants;

namespace Stockama.Tests.Users;

public class ResetUserOneTimePasswordCommandHandlerTests
{
   private readonly Mock<IResourceManager> _resourceManagerMock;
   private readonly Mock<IRepository<User>> _userRepositoryMock;
   private readonly Mock<IRepository<Company>> _companyRepositoryMock;
   private readonly Mock<IPasswordHasher> _passwordHasherMock;
   private readonly Mock<IQueueManager> _queueManagerMock;

   public ResetUserOneTimePasswordCommandHandlerTests()
   {
      _resourceManagerMock = new Mock<IResourceManager>();
      _userRepositoryMock = new Mock<IRepository<User>>();
      _companyRepositoryMock = new Mock<IRepository<Company>>();
      _passwordHasherMock = new Mock<IPasswordHasher>();
      _queueManagerMock = new Mock<IQueueManager>();
   }

   [Fact]
   public async Task HandleAsync_ShouldResetPassword_AndPublishQueue_WhenRequesterIsSuperAdmin()
   {
      var requesterId = Guid.NewGuid();
      var companyId = Guid.NewGuid();
      var targetUserId = Guid.NewGuid();
      QueueTemplateMessageRequest? queueRequest = null;

      _userRepositoryMock
         .SetupSequence(q => q.GetActiveAsync(It.IsAny<Expression<Func<User, bool>>>()))
         .ReturnsAsync(new User
         {
            Id = requesterId,
            IsSuperAdmin = true,
            FirstName = "Super",
            LastName = "Admin",
            Username = "sa",
            Email = "sa@x.com",
            PasswordHash = [],
            PasswordSalt = []
         })
         .ReturnsAsync(new User
         {
            Id = targetUserId,
            CompanyId = companyId,
            LanguageId = Guid.Parse(ApplicationContants.DefaultLanguageId),
            FirstName = "Tenant",
            LastName = "Admin",
            Username = "tenant.admin",
            Email = "tenant@x.com",
            PasswordHash = [1],
            PasswordSalt = [2]
         });

      _passwordHasherMock
         .Setup(q => q.HashPassword(It.IsAny<string>()))
         .Returns((new byte[] { 9, 9 }, new byte[] { 8, 8 }));

      _userRepositoryMock
         .Setup(q => q.UpdateAsync(It.IsAny<User>(), requesterId))
         .ReturnsAsync(true);

      _companyRepositoryMock
         .Setup(q => q.GetByIdActiveAsync(companyId))
         .ReturnsAsync(new Company { Id = companyId, Name = "ACME", CompanyCode = "acme", Description = "", LogoUrl = "", WebsiteUrl = "" });

      _queueManagerMock
         .Setup(q => q.EnqueueTemplateMessageAsync(It.IsAny<QueueTemplateMessageRequest>(), It.IsAny<CancellationToken>()))
         .Callback<QueueTemplateMessageRequest, CancellationToken>((request, _) => queueRequest = request)
         .Returns(Task.CompletedTask);

      var sut = new ResetUserOneTimePasswordCommandHandler(
         _resourceManagerMock.Object,
         _userRepositoryMock.Object,
         _companyRepositoryMock.Object,
         _passwordHasherMock.Object,
         _queueManagerMock.Object);

      var result = await sut.HandleAsync(new ResetUserOneTimePasswordCommand
      {
         UserId = requesterId,
         TargetUserId = targetUserId
      });

      Assert.True(result.IsSuccess);
      Assert.NotNull(queueRequest);
      Assert.Equal(QueueConstants.OneTimePasswordNotificationQueueName, queueRequest.QueueName);
      Assert.Equal(MessageTemplateConstants.TenantAdminOneTimePasswordTemplateKey, queueRequest.TemplateKey);
      Assert.Equal("tenant@x.com", queueRequest.Recipient);
      Assert.True(queueRequest.TemplateValues.ContainsKey("OneTimePassword"));
   }
}
