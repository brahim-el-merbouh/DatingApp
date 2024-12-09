using System;
using API.DTOs;
using API.Entities;

namespace API.Data.Repositories.Interfaces;

public interface IUserRepository : IRepositoryBase<AppUser>
{
    Task<MemberDto?> GetMemberAsync(string username);
    Task<IEnumerable<MemberDto>> GetAllMembersAsync();
}
