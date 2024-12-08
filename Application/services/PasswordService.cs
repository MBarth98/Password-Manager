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

        public async Task DeletePasswordAsync(int passwordId, int userId)
        {
            var entry = await _passwordRepository.GetByIdAsync(passwordId);
            if (entry == null || entry.UserId != userId)
            {
                throw new Exception("Password entry not found or not authorized.");
            }

            await _passwordRepository.DeleteAsync(entry);
        }

        public async Task UpdatePasswordAsync(int passwordId, int userId, AddPasswordDto updateDto)
        {
            var entry = await _passwordRepository.GetByIdAsync(passwordId);
            if (entry == null || entry.UserId != userId)
            {
                throw new Exception("Password entry not found or not authorized.");
            }

            entry.Website = updateDto.Website;
            entry.Username = updateDto.Username;
            entry.Salt = updateDto.Salt;
            entry.EncryptedPassword = updateDto.EncryptedPassword;

            await _passwordRepository.UpdateAsync(entry);
        }


        public async Task<List<PasswordEntry>> GetPasswordsAsync(int userId)
        {
            return await _passwordRepository.GetPasswordsByUserIdAsync(userId);
        }
    }
}
