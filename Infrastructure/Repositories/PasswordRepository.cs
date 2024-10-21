using System.Linq.Expressions;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class PasswordRepository(ApplicationDbContext context) : IRepository<Password>
{
    public async Task<Password> GetByIdAsync(int id)
    {
        return await context.Passwords
            .FirstOrDefaultAsync(c => c.Id == id) ?? throw new Exception();
    }

    public Task<IEnumerable<Password>> GetAllAsync()
    {
        throw new InvalidDataException();
    }

    public async Task<IEnumerable<Password>> FindAsync(Expression<Func<Password, bool>> predicate)
    {
        return await context.Passwords
            .Where(predicate)
            .ToListAsync();
    }
    
    public async Task<Password> AddAsync(Password entity)
    {
        context.Passwords.Add(entity);
        await context.SaveChangesAsync();
        return entity;
    }

    public void Remove(Password entity)
    {
        context.Passwords.Remove(entity);
        context.SaveChanges();
    }

    public void Update(Password entity)
    {
        context.Passwords.Update(entity);
        context.SaveChanges();
    }
}