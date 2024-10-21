using Microsoft.AspNetCore.Identity;

namespace Domain;

public class User : IdentityUser
{
    public ICollection<Password> Passwords { get; set; }
}