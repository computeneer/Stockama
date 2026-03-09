using System.Linq.Expressions;
using Moq;
using Stockama.Application.MessageTemplates.Commands.UpdateMessageTemplateCommand;
using Stockama.Core.Data;
using Stockama.Core.Resources;
using Stockama.Data.Domain;

namespace Stockama.Tests.MessageTemplates;

public class UpdateMessageTemplateCommandHandlerTests
{
   private readonly Mock<IResourceManager> _resourceManagerMock;
   private readonly Mock<IRepository<MessageTemplate>> _messageTemplateRepositoryMock;
   private readonly Mock<IRepository<User>> _userRepositoryMock;

   public UpdateMessageTemplateCommandHandlerTests()
   {
      _resourceManagerMock = new Mock<IResourceManager>();
      _messageTemplateRepositoryMock = new Mock<IRepository<MessageTemplate>>();
      _userRepositoryMock = new Mock<IRepository<User>>();
   }

   [Fact]
   public async Task HandleAsync_ShouldUpdateTemplate_WhenTemplateExists()
   {
      var userId = Guid.NewGuid();
      var languageId = Guid.NewGuid();

      var template = new MessageTemplate
      {
         TemplateKey = "tenant_admin_one_time_password",
         LanguageId = languageId,
         Subject = "Old",
         Body = "Old body"
      };

      _userRepositoryMock
         .Setup(q => q.GetActiveAsync(It.IsAny<Expression<Func<User, bool>>>() ))
         .ReturnsAsync(new User
         {
            Id = userId,
            IsSuperAdmin = true,
            FirstName = "Super",
            LastName = "Admin",
            Username = "sa",
            Email = "sa@x.com",
            PasswordHash = [],
            PasswordSalt = []
         });

      _messageTemplateRepositoryMock
         .Setup(q => q.GetActiveAsync(It.IsAny<Expression<Func<MessageTemplate, bool>>>() ))
         .ReturnsAsync(template);

      _messageTemplateRepositoryMock
         .Setup(q => q.UpdateAsync(template, userId))
         .ReturnsAsync(true);

      var sut = new UpdateMessageTemplateCommandHandler(
         _resourceManagerMock.Object,
         _messageTemplateRepositoryMock.Object,
         _userRepositoryMock.Object);

      var result = await sut.HandleAsync(new UpdateMessageTemplateCommand
      {
         UserId = userId,
         TemplateLanguageId = languageId,
         TemplateKey = "Tenant_Admin_One_Time_Password",
         Subject = "New",
         Body = "<b>New body</b>"
      });

      Assert.True(result.IsSuccess);
      Assert.Equal("New", template.Subject);
      Assert.Equal("<b>New body</b>", template.Body);
   }

   [Fact]
   public async Task HandleAsync_ShouldReturnNotFound_WhenTemplateMissing()
   {
      var userId = Guid.NewGuid();

      _userRepositoryMock
         .Setup(q => q.GetActiveAsync(It.IsAny<Expression<Func<User, bool>>>() ))
         .ReturnsAsync(new User
         {
            Id = userId,
            IsSuperAdmin = true,
            FirstName = "Super",
            LastName = "Admin",
            Username = "sa",
            Email = "sa@x.com",
            PasswordHash = [],
            PasswordSalt = []
         });

      _messageTemplateRepositoryMock
         .Setup(q => q.GetActiveAsync(It.IsAny<Expression<Func<MessageTemplate, bool>>>() ))
         .ReturnsAsync((MessageTemplate)null!);

      var sut = new UpdateMessageTemplateCommandHandler(
         _resourceManagerMock.Object,
         _messageTemplateRepositoryMock.Object,
         _userRepositoryMock.Object);

      var result = await sut.HandleAsync(new UpdateMessageTemplateCommand
      {
         UserId = userId,
         TemplateLanguageId = Guid.NewGuid(),
         TemplateKey = "tenant_admin_one_time_password",
         Subject = "New",
         Body = "Body"
      });

      Assert.False(result.IsSuccess);
      Assert.Equal("404", result.Status);
   }
}
