using Stockama.Helper.Utils;

namespace Stockama.Tests.Utils;

public class StringHelperTests
{
   [Theory]
   [InlineData(0)]
   [InlineData(1)]
   [InlineData(6)]
   [InlineData(32)]
   public void GenerateNumericCode_ShouldReturnOnlyDigits_WithRequestedLength(int length)
   {
      var code = StringHelper.GenerateNumericCode(length);

      Assert.Equal(length, code.Length);
      Assert.All(code, c => Assert.InRange(c, '0', '9'));
   }

   [Theory]
   [InlineData("user@example.com")]
   [InlineData("name.surname+tag@sub.domain.com")]
   public void GetEmailOrEmpty_ShouldReturnSameValue_ForValidEmail(string email)
   {
      var result = StringHelper.GetEmailOrEmpty(email);

      Assert.Equal(email, result);
   }

   [Theory]
   [InlineData("")]
   [InlineData("invalid-email")]
   [InlineData("user@")]
   [InlineData("user@example")]
   public void GetEmailOrEmpty_ShouldReturnEmpty_ForInvalidEmail(string email)
   {
      var result = StringHelper.GetEmailOrEmpty(email);

      Assert.Equal(string.Empty, result);
   }

   [Theory]
   [InlineData("05551234567")]
   [InlineData("+905551234567")]
   [InlineData("5551234567")]
   public void GetMobileOrEmpty_ShouldReturnSameValue_ForValidMobile(string phone)
   {
      var result = StringHelper.GetMobileOrEmpty(phone);

      Assert.Equal(phone, result);
   }

   [Theory]
   [InlineData("")]
   [InlineData("123456")]
   [InlineData("+441234567890")]
   [InlineData("0555123456A")]
   public void GetMobileOrEmpty_ShouldReturnEmpty_ForInvalidMobile(string phone)
   {
      var result = StringHelper.GetMobileOrEmpty(phone);

      Assert.Equal(string.Empty, result);
   }
}
