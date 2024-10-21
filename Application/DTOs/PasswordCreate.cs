namespace Application.DTOs;

public class PasswordCreate
{
    public string ServicePassword { get; set; }
    public string ServiceUsername { get; set; }
    public string ServiceName { get; set; }
    
    public string MasterPassword { get; set; }
}