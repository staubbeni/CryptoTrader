using Crypto.Data;
using Crypto.DTOs;
using Crypto.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Crypto.Controllers;

[Route("api/users")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly CryptoDbContext _context;

    public UsersController(CryptoDbContext context)
    {
        _context = context;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(UserRegisterDto dto)
    {
        if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
            return BadRequest("Email already exists.");

        var user = new User
        {
            Username = dto.Username,
            Email = dto.Email,
            Password = dto.Password 
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

       
        var wallet = new Wallet
        {
            UserId = user.Id,
            Balance = 1000.00 
        };

        _context.Wallets.Add(wallet);
        await _context.SaveChangesAsync();

        return Ok(new { user.Id, user.Username, user.Email });
    }
    [HttpGet("{userId}")]
    public async Task<IActionResult> GetUser(int userId)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
            return NotFound("User not found.");

        return Ok(new UserResponseDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email
        });
    }

    
    [HttpPut("{userId}")]
    public async Task<IActionResult> UpdateUser(int userId, UserUpdateDto dto)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
            return NotFound("User not found.");

        
        if (dto.Email != user.Email && await _context.Users.AnyAsync(u => u.Email == dto.Email && u.Id != userId))
            return BadRequest("Email already exists.");

        
        user.Username = dto.Username;
        user.Email = dto.Email;
        if (!string.IsNullOrEmpty(dto.Password)) 
            user.Password = dto.Password; 

        await _context.SaveChangesAsync();

        return Ok(new UserResponseDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email
        });
    }

    
    [HttpDelete("{userId}")]
    public async Task<IActionResult> DeleteUser(int userId)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
            return NotFound("User not found.");

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}