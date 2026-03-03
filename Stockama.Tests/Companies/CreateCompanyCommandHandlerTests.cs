using Moq;
using Stockama.Application.Companies.Command.CreateCompanyCommand;
using Stockama.Core.Resources;
using Stockama.Core.Tenants;
using Stockama.Core.Tenants.Models;

namespace Stockama.Tests.Companies;

public class CreateCompanyCommandHandlerTests
{
   private readonly Mock<IResourceManager> _resourceManagerMock;
   private readonly Mock<ITenantProvisionManager> _tenantProvisionManagerMock;

   public CreateCompanyCommandHandlerTests()
   {
      _resourceManagerMock = new Mock<IResourceManager>();
      _tenantProvisionManagerMock = new Mock<ITenantProvisionManager>();
   }

   [Fact]
   public async Task HandleAsync_ShouldProvisionTenant_AndReturnSuccess()
   {
      TenantProvisionRequest capturedRequest = null;

      _tenantProvisionManagerMock
         .Setup(q => q.ProvisionTenantAsync(It.IsAny<TenantProvisionRequest>(), It.IsAny<CancellationToken>()))
         .Callback<TenantProvisionRequest, CancellationToken>((request, _) => capturedRequest = request)
         .ReturnsAsync(new TenantProvisionResult(Guid.NewGuid(), Guid.NewGuid(), "admin.acme", "admin@acme.local"));

      var sut = new CreateCompanyCommandHandler(_resourceManagerMock.Object, _tenantProvisionManagerMock.Object);

      var result = await sut.HandleAsync(new CreateCompanyCommand
      {
         UserId = Guid.NewGuid(),
         Name = "ACME",
         Description = "desc",
         LogoUrl = "logo",
         WebsiteUrl = "site",
         CompanyCode = "acme",
         AdminFirstName = "Tenant",
         AdminLastName = "Admin",
         AdminEmail = "admin@acme.local"
      });

      Assert.True(result.IsSuccess);
      Assert.Equal("ACME", capturedRequest.CompanyName);
      Assert.Equal("acme", capturedRequest.CompanyCode);
   }
}
