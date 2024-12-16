using System.Security.Claims;
using API.Data.Repositories.Interfaces;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;
[Authorize]
public class UsersController(IUserService _userService, 
    IPhotoService _photoService) : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers()
    {
        var users = await _userService.GetAllUsers();

        return Ok(users);
    }
    [HttpGet("{username}")]
    public async Task<ActionResult<MemberDto>> GetUser(string username)
    {
        var user = await _userService.GetMember(username);

        if (user == null) return NotFound();

        return user;
    }
    [HttpPut]
    public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
    {
        var user = await _userService.GetUser(User.GetUserName());

        if (user == null) return BadRequest("Could not find user");

        var updateUser = await _userService.UpdateUser(user, memberUpdateDto);

        if (updateUser) return NoContent();

        return BadRequest("Failed to update the user");
    }

    [HttpPost("add-photo")]
    public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
    {
        var user = await _userService.GetUser(User.GetUserName());

        if (user == null) return BadRequest("user not found");

        var result = await _photoService.AddPhotoAsync(file);

        if (result.Error != null) return BadRequest(result.Error.Message);

        var photo = await _userService.AddPhotoForUser(user, result);

        if (photo.Id > 0) 
            return CreatedAtAction(nameof(GetUser),new {username = user.UserName}, photo);

        return BadRequest("Problem adding photo");
    }

    [HttpPut("set-main-photo/{photoId:int}")]
    public async Task<ActionResult> SetMainPhoto(int PhotoId)
    {
         var user = await _userService.GetUserWithPhotos(User.GetUserName());

        if (user == null) return BadRequest("user not found");

        var photo = user.Photos.FirstOrDefault(x => x.Id == PhotoId);

        if (photo == null || photo.IsMain) return BadRequest("Cannot use this as main photo!");

        if (await _userService.setMainPhotoForUser(user, photo)) return NoContent();

        return BadRequest("Problem setting main photo!");
    }

    [HttpDelete("delete-photo/{photoId:int}")]
    public async Task<ActionResult> DeletePhoto(int photoId)
    {
        var user = await _userService.GetUserWithPhotos(User.GetUserName());

        if (user == null) return BadRequest("user not found");

        var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

        if (photo == null || photo.IsMain) return BadRequest("This photo can not be deleted!");

        if (photo.PublicId != null)
        {
            var result = await _photoService.DeletePhotoAsync(photo.PublicId);
            if (result.Error != null) return BadRequest(result.Error.Message);
        }
        

        if (await _userService.DeletePhoto(user, photo)) return Ok();

        return BadRequest("Problem deleting photo");
    }
}
