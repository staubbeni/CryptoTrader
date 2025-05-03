using Crypto.Data;
using Crypto.DTOs;
using Crypto.Models;
using Microsoft.EntityFrameworkCore;

namespace Crypto.Services;

public interface IUserService
{
    Task<UserResponseDto> RegisterAsync(UserRegisterDto dto);
    Task<UserResponseDto> GetUserAsync(int userId);
    Task<UserResponseDto> UpdateUserAsync(int userId, UserUpdateDto dto);
    Task DeleteUserAsync(int userId);
}

public class UserService : IUserService
{
    private readonly CryptoDbContext _context;

    public UserService(CryptoDbContext context)
    {
        _context = context;
    }

    public async Task<UserResponseDto> RegisterAsync(UserRegisterDto dto)
    {
        if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
            throw new InvalidOperationException("Email already exists.");

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

        return new UserResponseDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email
        };
    }

    public async Task<UserResponseDto> GetUserAsync(int userId)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
            throw new KeyNotFoundException("User not found.");

        return new UserResponseDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email
        };
    }

    public async Task<UserResponseDto> UpdateUserAsync(int userId, UserUpdateDto dto)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
            throw new KeyNotFoundException("User not found.");

        if (dto.Email != user.Email && await _context.Users.AnyAsync(u => u.Email == dto.Email && u.Id != userId))
            throw new InvalidOperationException("Email already exists.");

        user.Username = dto.Username;
        user.Email = dto.Email;
        if (!string.IsNullOrEmpty(dto.Password))
            user.Password = dto.Password;

        await _context.SaveChangesAsync();

        return new UserResponseDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email
        };
    }

    public async Task DeleteUserAsync(int userId)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
            throw new KeyNotFoundException("User not found.");

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
    }
}