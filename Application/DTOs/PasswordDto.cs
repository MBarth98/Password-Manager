namespace Application.DTOs;

public class PasswordDto
{
    public string Website { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty; // Decrypted password
}
