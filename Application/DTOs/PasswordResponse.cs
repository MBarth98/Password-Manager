namespace Application.DTOs;

public class PasswordResponse
{
    public int Id { get; set; }
    public string ServiceName { get; set; }
    /// <summary>
    /// either the encrypted password or the decrypted password depending on the context
    /// </summary>
    public string ServicePassword { get; set; }
    
    public string ServiceUsername { get; set; }
}