using Domain;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class PasswordRepository : IPasswordRepository
{
    private readonly ApplicationDbContext _context;

    public PasswordRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddPasswordAsync(PasswordEntry passwordEntry)
    {
        _context.PasswordEntries.Add(passwordEntry);
        await _context.SaveChangesAsync();
    }

    public async Task<List<PasswordEntry>> GetPasswordsByUserIdAsync(int userId)
    {
        return await _context.PasswordEntries
            .Where(p => p.UserId == userId)
            .ToListAsync();
    }
}
