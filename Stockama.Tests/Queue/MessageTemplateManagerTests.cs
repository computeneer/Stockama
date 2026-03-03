using System.Linq.Expressions;
using Moq;
using Stockama.Core.Data;
using Stockama.Core.Exeptions;
using Stockama.Core.Queue;
using Stockama.Data.Domain;
using Stockama.Helper.Constants;

namespace Stockama.Tests.Queue;

public class MessageTemplateManagerTests
{
   private readonly Mock<IRepository<MessageTemplate>> _templateRepositoryMock;

   public MessageTemplateManagerTests()
   {
      _templateRepositoryMock = new Mock<IRepository<MessageTemplate>>();
   }

   [Fact]
   public async Task RenderAsync_ShouldUseDatabaseTemplate_WhenTemplateExists()
   {
      var languageId = Guid.NewGuid();

      _templateRepositoryMock
         .Setup(q => q.GetActiveAsync(It.IsAny<Expression<Func<MessageTemplate, bool>>>()))
         .ReturnsAsync(new MessageTemplate
         {
            TemplateKey = "tenant_admin_one_time_password",
            LanguageId = languageId,
            Subject = "Hello {{FirstName}}",
            Body = "Code: {{OneTimePassword}}"
         });

      var sut = new MessageTemplateManager(_templateRepositoryMock.Object);

      var result = await sut.RenderAsync(
         "tenant_admin_one_time_password",
         languageId,
         new Dictionary<string, string>
         {
            ["FirstName"] = "John",
            ["OneTimePassword"] = "123456"
         });

      Assert.Equal("Hello John", result.Subject);
      Assert.Equal("Code: 123456", result.Body);
   }

   [Fact]
   public async Task RenderAsync_ShouldUseDefaultTemplate_WhenDatabaseTemplateDoesNotExist()
   {
      _templateRepositoryMock
         .Setup(q => q.GetActiveAsync(It.IsAny<Expression<Func<MessageTemplate, bool>>>()))
         .ReturnsAsync((MessageTemplate)null);

      var sut = new MessageTemplateManager(_templateRepositoryMock.Object);

      var result = await sut.RenderAsync(
         MessageTemplateConstants.TenantAdminOneTimePasswordTemplateKey,
         Guid.NewGuid(),
         new Dictionary<string, string>
         {
            ["FirstName"] = "Jane",
            ["LastName"] = "Doe",
            ["CompanyName"] = "ACME",
            ["Username"] = "admin.acme",
            ["OneTimePassword"] = "999999"
         });

      Assert.Contains("Jane Doe", result.Body);
      Assert.Contains("999999", result.Body);
   }

   [Fact]
   public async Task RenderAsync_ShouldThrowNotFound_WhenTemplateUnknown()
   {
      _templateRepositoryMock
         .Setup(q => q.GetActiveAsync(It.IsAny<Expression<Func<MessageTemplate, bool>>>()))
         .ReturnsAsync((MessageTemplate)null);

      var sut = new MessageTemplateManager(_templateRepositoryMock.Object);

      await Assert.ThrowsAsync<HttpNotFoundExeption>(() =>
         sut.RenderAsync("unknown-template", Guid.NewGuid(), new Dictionary<string, string>()));
   }

   [Fact]
   public async Task RenderAsync_ShouldFallbackToDefaultEnglishLanguage_WhenRequestedLanguageTemplateMissing()
   {
      var englishLanguageId = Guid.Parse(ApplicationContants.DefaultLanguageId);

      _templateRepositoryMock
         .SetupSequence(q => q.GetActiveAsync(It.IsAny<Expression<Func<MessageTemplate, bool>>>()))
         .ReturnsAsync((MessageTemplate)null)
         .ReturnsAsync(new MessageTemplate
         {
            TemplateKey = MessageTemplateConstants.TenantAdminOneTimePasswordTemplateKey,
            LanguageId = englishLanguageId,
            Subject = "EN Subject {{FirstName}}",
            Body = "EN Body {{OneTimePassword}}"
         });

      var sut = new MessageTemplateManager(_templateRepositoryMock.Object);

      var result = await sut.RenderAsync(
         MessageTemplateConstants.TenantAdminOneTimePasswordTemplateKey,
         Guid.NewGuid(),
         new Dictionary<string, string>
         {
            ["FirstName"] = "Jane",
            ["OneTimePassword"] = "123123"
         });

      Assert.Equal("EN Subject Jane", result.Subject);
      Assert.Equal("EN Body 123123", result.Body);
   }
}
