namespace Stockama.Core.Security;


public interface IPasswordHasher
{
   (byte[], byte[]) HashPassword(string password);
   bool VerifyPassword(string password, byte[] salt, byte[] hash);
}