
namespace Domain.Interfaces;

public interface IPasswordRepository
{
    Task AddPasswordAsync(PasswordEntry passwordEntry);
    Task<List<PasswordEntry>> GetPasswordsByUserIdAsync(int userId);
}
