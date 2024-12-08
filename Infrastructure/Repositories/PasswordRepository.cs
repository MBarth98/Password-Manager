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
    
    public async Task<PasswordEntry?> GetByIdAsync(int passwordId)
    {
        return await _context.PasswordEntries.FindAsync(passwordId);
    }

    public async Task UpdateAsync(PasswordEntry entry)
    {
        _context.PasswordEntries.Update(entry);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(PasswordEntry entry)
    {
        _context.PasswordEntries.Remove(entry);
        await _context.SaveChangesAsync();
    }

    public async Task<List<PasswordEntry>> GetPasswordsByUserIdAsync(int userId)
    {
        return await _context.PasswordEntries
            .Where(p => p.UserId == userId)
            .ToListAsync();
    }
}
