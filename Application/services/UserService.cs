using Application.DTOs;
using Domain;
using Domain.Interfaces;

namespace Application.Services;

public class UserService
{
    private readonly IUserRepository _userRepository;
    private readonly TokenService _tokenService;

    public UserService(IUserRepository userRepository, TokenService tokenService)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
    }

    public async Task RegisterAsync(UserDto userDto)
    {
        var existingUser = await _userRepository.GetByEmailAsync(userDto.Email);
        if (existingUser != null)
        {
            throw new Exception("User already exists.");
        }

        var user = new User
        {
            Email = userDto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(userDto.Password)
        };

        await _userRepository.AddAsync(user);
    }

    public async Task<string> AuthenticateAsync(UserDto userLoginDto)
    {
        var user = await _userRepository.GetByEmailAsync(userLoginDto.Email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(userLoginDto.Password, user.PasswordHash))
        {
            throw new Exception("Invalid credentials.");
        }

        return _tokenService.GenerateToken(user);
    }
}
