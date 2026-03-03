using Stockama.Helper.Extensions;

namespace Stockama.Tests.Extensions;

public class GuidExtensionsTests
{
   [Fact]
   public void IsNullOrEmpty_ShouldReturnTrue_ForEmptyGuid()
   {
      var value = Guid.Empty;

      Assert.True(value.IsNullOrEmpty());
   }

   [Fact]
   public void IsNullOrEmpty_ShouldReturnFalse_ForNonEmptyGuid()
   {
      var value = Guid.NewGuid();

      Assert.False(value.IsNullOrEmpty());
   }

   [Fact]
   public void NullableIsNullOrEmpty_ShouldReturnTrue_ForNull()
   {
      Guid? value = null;

      Assert.True(value.IsNullOrEmpty());
   }

   [Fact]
   public void NullableIsNullOrEmpty_ShouldReturnTrue_ForEmptyGuid()
   {
      Guid? value = Guid.Empty;

      Assert.True(value.IsNullOrEmpty());
   }

   [Fact]
   public void NullableIsNullOrEmpty_ShouldReturnFalse_ForNonEmptyGuid()
   {
      Guid? value = Guid.NewGuid();

      Assert.False(value.IsNullOrEmpty());
   }
}
