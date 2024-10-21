namespace Application.helper;

public class PasswordGenerator
{
    
    public static string GeneratePassword(int minLength, int maxLength, bool useLowercase, bool useUppercase, bool useNumbers, bool useSpecialCharacters)
    {
        string lowerCase = "abcdefghijklmnopqrstuvwxyz";
        string upperCase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        string numbers = "1234567890";
        string specialCharacters = "!@#$%^&*()_+-=[]{}|;:,.<>?";

        char[] password = new char[maxLength];
        string characterSet = "";

        if (useLowercase)
        {
            characterSet += lowerCase;
        }

        if (useUppercase)
        {
            characterSet += upperCase;
        }

        if (useNumbers)
        {
            characterSet += numbers;
        }

        if (useSpecialCharacters)
        {
            characterSet += specialCharacters;
        }

        Random random = new Random();

        for (int i = 0; i < random.Next(minLength, maxLength); i++)
        {
            password[i] = characterSet[random.Next(characterSet.Length - 1)];
        }

        return new string(password);
    }
}