using System;
using API.Data.Repositories.Interfaces;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using CloudinaryDotNet.Actions;

namespace API.Services;

public class UserService(IUnitOfWork _unitOfWork, IMapper _mapper) : IUserService
{
    public async Task<IEnumerable<MemberDto>> GetAllUsers()
    {
        return await _unitOfWork.UserRepository.GetAllMembersAsync();
    }

    public async Task<MemberDto?> GetMember(string username)
    {
        if (string.IsNullOrEmpty(username)) throw new ArgumentException(nameof(username));

        return await _unitOfWork.UserRepository.GetMemberAsync(username);
    }

    public async Task<AppUser?> GetUser(string username)
    {
        if (string.IsNullOrEmpty(username)) throw new ArgumentException(nameof(username));

        return await _unitOfWork.UserRepository.FindByAsync( u => u.UserName == username);
    }
    public async Task<AppUser?> GetUserWithPhotos(string username)
    {
        if (string.IsNullOrEmpty(username)) throw new ArgumentException(nameof(username));

        return await _unitOfWork.UserRepository.FindByIncludingAsync( u => u.UserName == username, [u => u.Photos]);
    }
    public async Task<bool> UpdateUser(AppUser user, MemberUpdateDto userDto)
    {
        if (user == null) throw new Exception("User to update is required");
        if (userDto == null) throw new Exception("UserDto is required");

        _mapper.Map(userDto, user);
        _unitOfWork.UserRepository.Edit(user);

        return await _unitOfWork.Complete();
        
    }

    public async Task<PhotoDto> AddPhotoForUser(AppUser user, ImageUploadResult photDetails)
    {
        if (user == null) throw new Exception("User is required");
        if (photDetails.Error != null) throw new Exception("photDetails has errors");
        var photo = new Photo 
        {
            Url = photDetails.SecureUrl.AbsoluteUri,
            PublicId = photDetails.PublicId
        };

        user.Photos.Add(photo);

        await _unitOfWork.Complete();

        return _mapper.Map<Photo,PhotoDto>(photo);
    }

    public async Task<bool> setMainPhotoForUser(AppUser user, Photo photo)
    {
        if (user == null) throw new Exception("User is required");
        if(user.Photos == null) throw new Exception("User must have photos");
        if (photo == null) throw new Exception("Photo is required");

        photo.IsMain = true;
        var currentMain = user.Photos.FirstOrDefault(x => x.IsMain);
        if (currentMain != null) 
        {
            currentMain.IsMain = false;
            _unitOfWork.PhotoRepository.Edit(currentMain);
        }
        _unitOfWork.PhotoRepository.Edit(photo);

        return await _unitOfWork.Complete();
    }

    public async Task<bool> DeletePhoto(AppUser user, Photo photo)
    {
        if (user == null) throw new Exception("User is required");
        if(user.Photos == null) throw new Exception("User must have photos");
        if (photo == null) throw new Exception("Photo is required");

        user.Photos.Remove(photo);

        return await _unitOfWork.Complete();

    }
}
