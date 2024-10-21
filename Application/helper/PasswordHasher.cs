using System.Text;
using Konscious.Security.Cryptography;

namespace Application.helper;

public class PasswordHasher
{
    public static string Hash(string password, string salt, string secret)
    {
        byte[] saltByte = Encoding.UTF8.GetBytes(secret + salt);

        var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
        {
            Salt = saltByte,
            DegreeOfParallelism = 4,
            MemorySize = 65536,
            Iterations = 4
        };
        byte[] hash = argon2.GetBytes(32);
        
        return Convert.ToBase64String(hash);
    }
}