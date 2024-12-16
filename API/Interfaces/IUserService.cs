using System;
using API.DTOs;
using API.Entities;
using CloudinaryDotNet.Actions;

namespace API.Interfaces;

public interface IUserService
{
    Task<IEnumerable<MemberDto>> GetAllUsers();
    Task<MemberDto?> GetMember(string username);
    Task<AppUser?> GetUser(string username);
    Task<AppUser?> GetUserWithPhotos(string username);
    Task<bool> UpdateUser(AppUser user, MemberUpdateDto userDto);
    Task<PhotoDto> AddPhotoForUser(AppUser user, ImageUploadResult photDetails);
    Task<bool> setMainPhotoForUser(AppUser user, Photo photo);
    Task<bool> DeletePhoto(AppUser user, Photo photo);
}
