using System.Security.Cryptography;
using System.Text;

namespace Application.helper;

public class PasswordEncryption
{
    public static (string Cipher, string IV) Encrypt(string plainText, string key)
    {
        key = PasswordHasher.Hash(key, key, key);
        if (key.Length < 32)
            key += key;
        key = key.Substring(0, 32);
            
        using Aes aes = Aes.Create();
        aes.Key = Encoding.Default.GetBytes(key);
        
        aes.GenerateIV(); 

        ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

        using MemoryStream ms = new MemoryStream();
        using CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write);
        using (StreamWriter sw = new StreamWriter(cs))
        {
            sw.Write(plainText);
        }

        return (Convert.ToBase64String(ms.ToArray()), Convert.ToBase64String(aes.IV));
    }

    public static string Decrypt(string cipher, string key, string iv)
    {
        key = PasswordHasher.Hash(key, key, key);
        if (key.Length < 32)
            key += key;
        key = key.Substring(0, 32);
        
        using Aes aes = Aes.Create();
        aes.Key = Encoding.Default.GetBytes(key);
        aes.IV = Convert.FromBase64String(iv);
        
        ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

        using MemoryStream ms = new MemoryStream(Convert.FromBase64String(cipher));
        using CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
        using (StreamReader sr = new StreamReader(cs))
        {
            return sr.ReadToEnd();
        }
    }
}