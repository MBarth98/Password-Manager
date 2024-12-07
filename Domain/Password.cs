using System.ComponentModel.DataAnnotations.Schema;

namespace Domain;

public class PasswordEntry
{
    public int Id { get; set; }
    public string Website { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Salt { get; set; } = string.Empty;
    public string EncryptedPassword { get; set; } = string.Empty;

    public int UserId { get; set; }
    
    [ForeignKey(nameof(UserId))]
    public User User { get; set; }
}