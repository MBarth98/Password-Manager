﻿using System.Linq.Expressions;

namespace Infrastructure;

public interface IRepository<T> where T : class
{
    Task<T> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    
    Task<T> AddAsync(T entity);
    void Remove(T entity);
    void Update(T entity);
}