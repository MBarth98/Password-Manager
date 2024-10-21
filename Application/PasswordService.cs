using Application.helper;
using Domain;
using Infrastructure;

namespace Application;

public class PasswordService(IRepository<Password> commentRepository)
{
    public async Task<Password> CreateAsync(Password comment)
    {
        return await commentRepository.AddAsync(comment);
    }

    public async Task UpdateAsync(Password comment)
    {
        commentRepository.Update(comment);
    }

    public async Task DeleteAsync(int id)
    {
        var password = await commentRepository.GetByIdAsync(id);
        if (password != null)
        {
            commentRepository.Remove(password);
        }
    }
    public async Task<IEnumerable<Password>> GetAllByUserAsync(string userId)
    {
        return await commentRepository.FindAsync(password => password.UserId == userId);
    }
    public async Task<Password> GetByIdAsync(int id)
    {
        return await commentRepository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<Password>> GetAllAsync()
    {
        return await commentRepository.GetAllAsync();
    }
}