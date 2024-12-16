using System;
using API.Data.Repositories.Interfaces;
using API.Entities;

namespace API.Data.Repositories;

public class PhotoRepository(DataContext _context) : RepositoryBase<Photo>(_context), IPhotoRepository
{

}
