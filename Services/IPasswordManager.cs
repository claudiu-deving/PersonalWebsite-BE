using System.Security.Cryptography;

namespace ccsflowserver.Services;

public interface IPasswordManager
{
    (byte[] hash, byte[] salt) HashNewPassword(string password);
    byte[] HashPassword(string password, byte[] salt);
}

public class PasswordManager : IPasswordManager
{
    public (byte[] hash, byte[] salt) HashNewPassword(string password)
    {
        var salt = GenerateSalt();
        // PBKDF2 implementation
        var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA256);
        byte[] hash = pbkdf2.GetBytes(20);
        // Combine the salt and password bytes for storage
        byte[] hashBytes = new byte[36];
        Array.Copy(salt, 0, hashBytes, 0, 16);
        Array.Copy(hash, 0, hashBytes, 16, 20);
        return (hashBytes, salt);
    }
    public byte[] HashPassword(string password, byte[] salt)
    {

        // PBKDF2 implementation
        var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA256);
        byte[] hash = pbkdf2.GetBytes(20);
        // Combine the salt and password bytes for storage
        byte[] hashBytes = new byte[36];
        Array.Copy(salt, 0, hashBytes, 0, 16);
        Array.Copy(hash, 0, hashBytes, 16, 20);
        return hashBytes;
    }

    private byte[] GenerateSalt()
    {
        using(var rng = new RNGCryptoServiceProvider())
        {
            byte[] salt = new byte[16];
            rng.GetBytes(salt);
            return salt;
        }
    }
}