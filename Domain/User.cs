﻿using Microsoft.AspNetCore.Identity;

namespace Domain;

 public class User
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public List<PasswordEntry> PasswordEntries { get; set; } = new();
}