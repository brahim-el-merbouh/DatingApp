using System;
using System.Linq.Expressions;
using API.Entities;

namespace API.Data.Repositories.Interfaces;

public interface IRepositoryBase<T> where T: class, IEntityBase
{
    Task<IEnumerable<T>> GetAllIncludingAsync(params Expression<Func<T, object>>[] includeProperties);
    Task<IEnumerable<T>> GetAllAsync();
    Task<T?> GetSingleAsync(int id);
    Task<T?> GetSingleIncludingAsync(int id, params Expression<Func<T, object>>[] includeProperties);
    Task<T?> FindByAsync(Expression<Func<T, bool>> predicate);
    Task<T?> FindByIncludingAsync(Expression<Func<T, bool>> predicate, Expression<Func<T, object>>[] includeProperties);
    void Add(T entity);
    void Delete(T entity);
    void Edit(T entity);
}
