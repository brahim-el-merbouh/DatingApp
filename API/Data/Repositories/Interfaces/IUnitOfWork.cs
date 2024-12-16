using System;

namespace API.Data.Repositories.Interfaces;

public interface IUnitOfWork
{
    IUserRepository UserRepository {get;}
    IPhotoRepository PhotoRepository {get;}
    Task<bool> Complete();
    bool HasChanges();

}
