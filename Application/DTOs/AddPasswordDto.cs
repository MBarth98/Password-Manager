namespace Application.DTOs;

public class AddPasswordDto
{
    public string Website { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Salt { get; set; } = string.Empty;
    public string EncryptedPassword { get; set; } = string.Empty;
}