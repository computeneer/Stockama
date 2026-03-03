using System.Net;
using Stockama.Helper.Extensions;

namespace Stockama.Tests.Extensions;

public class HttpStatusCodeExtensionsTests
{
   [Theory]
   [InlineData(HttpStatusCode.OK, "200")]
   [InlineData(HttpStatusCode.BadRequest, "400")]
   [InlineData(HttpStatusCode.NotFound, "404")]
   public void ToIntString_ShouldReturnNumericString(HttpStatusCode statusCode, string expected)
   {
      var result = statusCode.ToIntString();

      Assert.Equal(expected, result);
   }
}
