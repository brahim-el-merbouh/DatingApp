using System;
using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AccountController(IUserService _userService, ITokenService _tokenService, IMapper _mapper) : BaseApiController
{
    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register(RegisterDto userInfo)
    {
        if (await UserExists(userInfo.Username))
            return BadRequest($"Username {userInfo.Username} already exists");

        
        using var hmac = new HMACSHA512();

        var user = _mapper.Map<AppUser>(userInfo);

        user.UserName = userInfo.Username.ToLower();
        user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(userInfo.Password));
        user.PasswordSalt = hmac.Key;
        
        await _userService.CreateUser(user);

        return new UserDto
        {
            Username = user.UserName,
            Token = _tokenService.CreateToken(user),
            KnownAs = user.KnownAs
        };
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto userInfo)
    {
        var user = await _userService.GetUserWithPhotos(userInfo.Username.ToLower());

        if (user == null) return Unauthorized("Invalid username");

        using var hmac = new HMACSHA512(user.PasswordSalt);

        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(userInfo.Password));

        for (int i = 0; i < computedHash.Length; i++)
        {
            if (computedHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid password");
        }

        return new UserDto
        {
            Username = user.UserName,
            KnownAs = user.KnownAs,
            Token = _tokenService.CreateToken(user),
            PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url
        };
    }
    [HttpGet("test-error")]
    public ActionResult TestExceptionMiddleware()
    {
        throw new Exception("This is for your error testing");
    }

    private async Task<bool> UserExists(string username)
    {
        var user = await _userService.GetUser(username.ToLower());
        return user != null;
    }
}
