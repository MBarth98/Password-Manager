
namespace Domain.Interfaces;

public interface IPasswordRepository
{
    Task AddPasswordAsync(PasswordEntry passwordEntry);
    Task<PasswordEntry?> GetByIdAsync(int passwordId);
    Task UpdateAsync(PasswordEntry entry);
    Task DeleteAsync(PasswordEntry entry);
    Task<List<PasswordEntry>> GetPasswordsByUserIdAsync(int userId);
}
