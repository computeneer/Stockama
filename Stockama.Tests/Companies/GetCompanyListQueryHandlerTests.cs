using System.Linq.Expressions;
using Moq;
using Stockama.Application.Companies.Query.GetCompanyListQuery;
using Stockama.Core.Data;
using Stockama.Core.Resources;
using Stockama.Data.Domain;

namespace Stockama.Tests.Companies;

public class GetCompanyListQueryHandlerTests
{
   private readonly Mock<IResourceManager> _resourceManagerMock;
   private readonly Mock<IRepository<Company>> _companyRepositoryMock;

   public GetCompanyListQueryHandlerTests()
   {
      _resourceManagerMock = new Mock<IResourceManager>();
      _companyRepositoryMock = new Mock<IRepository<Company>>();
   }

   [Fact]
   public async Task HandleAsync_ShouldReturnMappedActiveCompanies()
   {
      _companyRepositoryMock
         .Setup(q => q.AllActiveAsync())
         .ReturnsAsync(
         [
            new Company { Id = Guid.NewGuid(), Name = "A", CompanyCode = "a", Description = "d1", LogoUrl = "l1", WebsiteUrl = "w1" },
            new Company { Id = Guid.NewGuid(), Name = "B", CompanyCode = "b", Description = "d2", LogoUrl = "l2", WebsiteUrl = "w2" }
         ]);

      var sut = new GetCompanyListQueryHandler(_resourceManagerMock.Object, _companyRepositoryMock.Object);

      var result = await sut.HandleAsync(new GetCompanyListQuery());

      Assert.True(result.IsSuccess);
      Assert.Equal(2, result.Total);
      Assert.Equal(2, result.Data.Count());
      Assert.Contains(result.Data, q => q.Name == "A" && q.CompanyCode == "a");
   }
}
