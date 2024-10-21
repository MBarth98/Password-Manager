using System.Linq.Expressions;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class UserRepository(ApplicationDbContext context) : IRepository<User>
{
    public async Task<User> GetByIdAsync(int id)
    {
        return await context.Users
            .Include(u => u.Passwords) // Include articles authored by the user
            .FirstOrDefaultAsync(u => u.Id == id + "") ?? throw new InvalidOperationException();
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await context.Users
            .Include(u => u.Passwords) // Include articles authored by the user
            .ToListAsync();
    }

    public async Task<IEnumerable<User>> FindAsync(Expression<Func<User, bool>> predicate)
    {
        return await context.Users
            .Include(u => u.Passwords) // Include articles authored by the user
            .Where(predicate)
            .ToListAsync();
    }

    public async Task<User> AddAsync(User entity)
    {
        context.Users.Add(entity);
        await context.SaveChangesAsync();
        return entity;
    }

    public void Remove(User entity)
    {
        context.Users.Remove(entity);
        context.SaveChanges();
    }

    public void Update(User entity)
    {
        context.Users.Update(entity);
        context.SaveChanges();
    }
}