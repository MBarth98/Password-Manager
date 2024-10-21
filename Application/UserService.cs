using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Application.DTOs;
using AutoMapper;
using Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Application;

public class UserService(UserManager<User> userManager, IMapper mapper, IConfiguration configuration) 
{
    private readonly IConfiguration _configuration = configuration;

    public async Task<User> RegisterAsync(RegisterCreate user)
    {
        var create = mapper.Map<User>(user);

        create.PasswordHash = new PasswordHasher<User>().HashPassword(create, user.Password);
        
        var result = await userManager.CreateAsync(create);
        
        if (!result.Succeeded)
        {
            throw new ApplicationException("User registration failed.");
        }
        
        return create;
    }

    public async Task<string> AuthenticateAsync(LoginRequest login)
    {
        var user = await userManager.FindByNameAsync(login.Username);
        if (user == null || !await userManager.CheckPasswordAsync(user, login.Password))
        {
            throw new ApplicationException("Invalid username or password.");
        }

        // Generate JWT Token with permissions
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = "very_secret_key_very_secret_key_very_secret_key"u8.ToArray();
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.UserName ?? throw new InvalidOperationException()),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(2),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public async Task<User> GetByIdAsync(string id)
    {
        return await userManager.FindByIdAsync(id) ?? throw new InvalidOperationException();
    }
}

