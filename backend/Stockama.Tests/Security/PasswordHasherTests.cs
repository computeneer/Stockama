using Stockama.Core.Security;

namespace Stockama.Tests.Security;

public class PasswordHasherTests
{
   [Fact]
   public void HashPassword_ShouldCreateHashAndSalt_WithExpectedLengths()
   {
      var sut = new PasswordHasher();

      var (hash, salt) = sut.HashPassword("StrongPassword123!");

      Assert.NotNull(hash);
      Assert.NotNull(salt);
      Assert.Equal(64, hash.Length);
      Assert.Equal(32, salt.Length);
   }

   [Fact]
   public void VerifyPassword_ShouldReturnTrue_ForCorrectPassword()
   {
      var sut = new PasswordHasher();
      var password = "CorrectPassword!";
      var (hash, salt) = sut.HashPassword(password);

      var result = sut.VerifyPassword(password, salt, hash);

      Assert.True(result);
   }

   [Fact]
   public void VerifyPassword_ShouldReturnFalse_ForWrongPassword()
   {
      var sut = new PasswordHasher();
      var (hash, salt) = sut.HashPassword("CorrectPassword!");

      var result = sut.VerifyPassword("WrongPassword!", salt, hash);

      Assert.False(result);
   }
}
