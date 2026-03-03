using System.Linq.Expressions;
using Moq;
using Stockama.Application.MessageTemplates.Commands.CreateMessageTemplateCommand;
using Stockama.Core.Data;
using Stockama.Core.Resources;
using Stockama.Data.Domain;

namespace Stockama.Tests.MessageTemplates;

public class CreateMessageTemplateCommandHandlerTests
{
   private readonly Mock<IResourceManager> _resourceManagerMock;
   private readonly Mock<IRepository<MessageTemplate>> _messageTemplateRepositoryMock;
   private readonly Mock<IRepository<User>> _userRepositoryMock;

   public CreateMessageTemplateCommandHandlerTests()
   {
      _resourceManagerMock = new Mock<IResourceManager>();
      _messageTemplateRepositoryMock = new Mock<IRepository<MessageTemplate>>();
      _userRepositoryMock = new Mock<IRepository<User>>();
   }

   [Fact]
   public async Task HandleAsync_ShouldCreateTemplate_WhenRequesterIsSuperAdminAndTemplateNotExists()
   {
      var userId = Guid.NewGuid();
      var languageId = Guid.NewGuid();
      MessageTemplate createdTemplate = null;

      _userRepositoryMock
         .Setup(q => q.GetActiveAsync(It.IsAny<Expression<Func<User, bool>>>() ))
         .ReturnsAsync(new User { Id = userId, IsSuperAdmin = true, Username = "sa", Email = "sa@x.com", PasswordHash = [], PasswordSalt = [] });

      _messageTemplateRepositoryMock
         .Setup(q => q.AnyActiveAsync(It.IsAny<Expression<Func<MessageTemplate, bool>>>() ))
         .ReturnsAsync(false);

      _messageTemplateRepositoryMock
         .Setup(q => q.CreateBulkAsync(It.IsAny<List<MessageTemplate>>(), userId))
         .Callback<List<MessageTemplate>, Guid>((templates, _) => createdTemplate = templates.Single())
         .ReturnsAsync(true);

      var sut = new CreateMessageTemplateCommandHandler(
         _resourceManagerMock.Object,
         _messageTemplateRepositoryMock.Object,
         _userRepositoryMock.Object);

      var result = await sut.HandleAsync(new CreateMessageTemplateCommand
      {
         UserId = userId,
         TemplateLanguageId = languageId,
         TemplateKey = "Tenant_Admin_One_Time_Password",
         Subject = "Subject",
         Body = "<b>Body</b>"
      });

      Assert.True(result.IsSuccess);
      Assert.Equal("tenant_admin_one_time_password", createdTemplate.TemplateKey);
      Assert.Equal(languageId, createdTemplate.LanguageId);
      Assert.Equal("Subject", createdTemplate.Subject);
      Assert.Equal("<b>Body</b>", createdTemplate.Body);
   }

   [Fact]
   public async Task HandleAsync_ShouldReturnConflict_WhenTemplateAlreadyExists()
   {
      var userId = Guid.NewGuid();

      _userRepositoryMock
         .Setup(q => q.GetActiveAsync(It.IsAny<Expression<Func<User, bool>>>() ))
         .ReturnsAsync(new User { Id = userId, IsSuperAdmin = true, Username = "sa", Email = "sa@x.com", PasswordHash = [], PasswordSalt = [] });

      _messageTemplateRepositoryMock
         .Setup(q => q.AnyActiveAsync(It.IsAny<Expression<Func<MessageTemplate, bool>>>() ))
         .ReturnsAsync(true);

      var sut = new CreateMessageTemplateCommandHandler(
         _resourceManagerMock.Object,
         _messageTemplateRepositoryMock.Object,
         _userRepositoryMock.Object);

      var result = await sut.HandleAsync(new CreateMessageTemplateCommand
      {
         UserId = userId,
         TemplateLanguageId = Guid.NewGuid(),
         TemplateKey = "tenant_admin_one_time_password",
         Subject = "Subject",
         Body = "Body"
      });

      Assert.False(result.IsSuccess);
      Assert.Equal("409", result.Status);
   }
}
