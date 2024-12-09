using System;
using System.Linq.Expressions;
using API.Data;
using API.Data.Repositories.Interfaces;
using API.DTOs;
using API.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;
[Authorize]
public class UsersController(IUnitOfWork _unitOfWork) : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers()
    {
        var users = await _unitOfWork.UserRepository.GetAllMembersAsync();

        return Ok(users);
    }
    [HttpGet("{username}")]
    public async Task<ActionResult<MemberDto>> GetUser(string username)
    {
        var user = await _unitOfWork.UserRepository.GetMemberAsync(username);

        if (user == null) return NotFound();

        return user;
    }
}
