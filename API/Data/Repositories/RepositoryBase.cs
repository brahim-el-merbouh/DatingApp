using System;
using System.Linq.Expressions;
using API.Data.Repositories.Interfaces;
using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data.Repositories;

public class RepositoryBase<T>(DataContext _context) : IRepositoryBase<T> where T : class, IEntityBase
{
    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _context.Set<T>().ToListAsync();
    }

    public async Task<IEnumerable<T>> GetAllIncludingAsync(params Expression<Func<T, object>>[] includeProperties)
    {
        IQueryable<T> query = _context.Set<T>();
        foreach (var prop in includeProperties)
        {
            query = query.Include(prop);
        }
        return await query.ToListAsync();
    }

    public async Task<T?> GetSingleAsync(int id)
    {
        return await _context.Set<T>().FirstOrDefaultAsync();
    }

     public async Task<T?> GetSingleIncludingAsync(int id, params Expression<Func<T, object>>[] includeProperties)
    {
        IQueryable<T> query = _context.Set<T>();
        foreach (var includeProperty in includeProperties)
        {
            query = query.Include(includeProperty);
        }
        return await  query.FirstOrDefaultAsync();
    }

    public async Task<T?> FindByAsync(Expression<Func<T, bool>> predicate)
    {
        return await _context.Set<T>().Where(predicate).FirstOrDefaultAsync();
    }

    public async Task<T?> FindByIncludingAsync(Expression<Func<T, bool>> predicate, Expression<Func<T, object>>[] includeProperties)
    {
        IQueryable<T> query = _context.Set<T>().Where(predicate);
        foreach (var includeProperty in includeProperties)
        {
            query = query.Include(includeProperty);
        }
        return await query.FirstOrDefaultAsync();
    }
   
    public void Add(T entity)
    {
        _ = _context.Entry<T>(entity);
        _context.Set<T>().Add(entity);
    }

    public void Delete(T entity)
    {
        var entityEntry = _context.Entry<T>(entity);
        entityEntry.State = EntityState.Deleted;
    }

    public void Edit(T entity)
    {
        var entityEntry = _context.Entry<T>(entity);
        entityEntry.State = EntityState.Modified;
    }

}
