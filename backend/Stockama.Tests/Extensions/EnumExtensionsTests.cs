using System.ComponentModel;
using Stockama.Helper.Extensions;

namespace Stockama.Tests.Extensions;

public class EnumExtensionsTests
{
   private enum TestEnum
   {
      [Description("3f2504e0-4f89-11d3-9a0c-0305e82c3301")]
      WithGuidDescription = 1,

      [Description("Some text")]
      WithTextDescription = 2,

      WithoutDescription = 3
   }

   [Fact]
   public void GetGuid_ShouldParseGuid_FromDescriptionAttribute()
   {
      var result = TestEnum.WithGuidDescription.GetGuid();

      Assert.Equal(Guid.Parse("3f2504e0-4f89-11d3-9a0c-0305e82c3301"), result);
   }

   [Fact]
   public void GetGuid_ShouldReturnEmptyGuid_WhenDescriptionIsNotGuid()
   {
      var result = TestEnum.WithTextDescription.GetGuid();

      Assert.Equal(Guid.Empty, result);
   }

   [Fact]
   public void GetDescription_ShouldReturnDescription_WhenAttributeExists()
   {
      var result = TestEnum.WithTextDescription.GetDescription();

      Assert.Equal("Some text", result);
   }

   [Fact]
   public void GetDescription_ShouldReturnEmpty_WhenAttributeDoesNotExist()
   {
      var result = TestEnum.WithoutDescription.GetDescription();

      Assert.Equal(string.Empty, result);
   }
}
