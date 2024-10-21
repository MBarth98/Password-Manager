using System.ComponentModel.DataAnnotations.Schema;

namespace Domain;
using System;

public class Password
{
    public int Id { get; set; }
    public string UserId { get; set; }
    
    [ForeignKey(nameof(UserId))]
    public User User { get; set; }
    
    public string ServiceName { get; set; }
    public string ServiceUsername { get; set; }
    public string ServicePassword { get; set; }
    
    public string IV { get; set; }
}