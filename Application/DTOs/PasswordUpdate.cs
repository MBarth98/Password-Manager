namespace Application.DTOs;

public class PasswordUpdate
{
    public int Id { get; set; }
    public string ServiceName { get; set; }
    public string ServicePassword { get; set; }
    public string MasterPassword { get; set; }
}