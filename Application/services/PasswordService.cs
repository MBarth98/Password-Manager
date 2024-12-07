using Application.DTOs;
using Domain;
using Domain.Interfaces;

namespace Application.Services
{
    public class PasswordService
    {
        private readonly IPasswordRepository _passwordRepository;

        public PasswordService(IPasswordRepository passwordRepository)
        {
            _passwordRepository = passwordRepository;
        }

        public async Task AddPasswordAsync(int userId, AddPasswordDto addPasswordDto)
        {
            var passwordEntry = new PasswordEntry
            {
                Website = addPasswordDto.Website,
                Username = addPasswordDto.Username,
                Salt = addPasswordDto.Salt,
                EncryptedPassword = addPasswordDto.EncryptedPassword,
                UserId = userId
            };

            await _passwordRepository.AddPasswordAsync(passwordEntry);
        }

        public async Task<List<PasswordEntry>> GetPasswordsAsync(int userId)
        {
            return await _passwordRepository.GetPasswordsByUserIdAsync(userId);
        }
    }
}
