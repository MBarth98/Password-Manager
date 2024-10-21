namespace Application.DTOs;

public class GeneratePasswordRequest
{
    public int minLength { get; set; } 
    public int maxLength { get; set; }
    public bool useLowercase { get; set; }
    public bool useUppercase { get; set; }
    public bool useNumbers { get; set; }
    public bool useSpecialCharacters { get; set; }
    
}