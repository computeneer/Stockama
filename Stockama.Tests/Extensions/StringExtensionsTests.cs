using Stockama.Helper.Extensions;

namespace Stockama.Tests.Extensions;

public class StringExtensionsTests
{
   [Fact]
   public void GetImageUrl_ShouldReturnNull_WhenImageNameIsNull()
   {
      string imageName = null;

      var result = imageName.GetImageUrl("https://bucket/");

      Assert.Null(result);
   }

   [Fact]
   public void GetImageUrl_ShouldReturnNull_WhenImageNameIsEmpty()
   {
      var result = string.Empty.GetImageUrl("https://bucket/");

      Assert.Null(result);
   }

   [Fact]
   public void GetImageUrl_ShouldConcatenateBucketUrlAndImageName()
   {
      var result = "a.png".GetImageUrl("https://bucket/");

      Assert.Equal("https://bucket/a.png", result);
   }
}
