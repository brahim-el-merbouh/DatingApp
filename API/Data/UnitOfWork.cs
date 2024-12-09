using System;
using API.Data.Repositories.Interfaces;

namespace API.Data;

public class UnitOfWork(DataContext _context, IUserRepository _userRepository) : IUnitOfWork
{
    public IUserRepository UserRepository => _userRepository;

    public async Task<bool> Complete()
    {
        return await _context.SaveChangesAsync() > 0;
    }

    public bool HasChanges()
    {
        return _context.ChangeTracker.HasChanges();
    }
}
