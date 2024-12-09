using System;
using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AccountController(DataContext context, ITokenService tokenService) : BaseApiController
{
    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register(RegisterDto userInfo)
    {
        if (await UserExists(userInfo.Username))
            return BadRequest($"Username {userInfo.Username} already exists");

        return Ok();
        // using var hmac = new HMACSHA512();

        // var user = new AppUser
        // {
        //     UserName = userInfo.Username.ToLower(),
        //     PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(userInfo.Password)),
        //     PasswordSalt = hmac.Key
        // };

        // context.Users.Add(user);
        // await context.SaveChangesAsync();

        // return new UserDto
        // {
        //     Username = user.UserName,
        //     Token = tokenService.CreateToken(user)
        // };
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto userInfo)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.UserName == userInfo.Username.ToLower());

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
            Token = tokenService.CreateToken(user)
        };
    }
    [HttpGet("test-error")]
    public ActionResult TestExceptionMiddleware()
    {
        throw new Exception("This is for your error testing");
    }

    private async Task<bool> UserExists(string username)
    {
        return await context.Users.AnyAsync( u => u.UserName.ToLower() == username.ToLower());
    }
}
