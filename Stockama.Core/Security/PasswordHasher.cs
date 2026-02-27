using System.Security.Cryptography;

namespace Stockama.Core.Security;

public class PasswordHasher : IPasswordHasher
{
   private readonly int _saltSize = 32;
   private readonly int _hashSize = 64;
   private readonly int _iterations = 5000;
   private readonly HashAlgorithmName _hashAlgorithmName = HashAlgorithmName.SHA256;

   public (byte[], byte[]) HashPassword(string password)
   {
      byte[] salt = RandomNumberGenerator.GetBytes(_saltSize);
      byte[] hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, _iterations, _hashAlgorithmName, _hashSize);

      return (hash, salt);
   }

   public bool VerifyPassword(string password, byte[] salt, byte[] hash)
   {
      byte[] inputPasswordHash = Rfc2898DeriveBytes.Pbkdf2(password, salt, _iterations, _hashAlgorithmName, _hashSize);

      return CryptographicOperations.FixedTimeEquals(inputPasswordHash, hash);
   }
}