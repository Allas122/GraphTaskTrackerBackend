using GraphTaskTrackerBackend.Application.DTO;
using GraphTaskTrackerBackend.Application.Exceptions;
using GraphTaskTrackerBackend.Application.Mappers;
using GraphTaskTrackerBackend.Application.Services.Abstractions;
using GraphTaskTrackerBackend.Domain.Entities;
using GraphTaskTrackerBackend.Infrastructure.DataBase;
using Microsoft.EntityFrameworkCore;

namespace GraphTaskTrackerBackend.Application.Services.Implementations;

public class UserService : IUserService
{
    private readonly DatabaseContext _databaseContext;
    private readonly ILogger<UserService> _logger;

    public UserService(
        DatabaseContext databaseContext,
        ILogger<UserService> logger)
    {
        _databaseContext = databaseContext;
        _logger = logger;
    }

    public async Task<UserDto> CreateUserAsync(VerifyUserDto user)
    {
        var userEntity = new User
        {
            Name = user.Name,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.Password)
        };
        if (await _databaseContext.Users.AnyAsync(u => u.Name == userEntity.Name))
            throw new AlreadyExistsException("User already exists");
        await _databaseContext.AddAsync(userEntity);
        await _databaseContext.SaveChangesAsync();
        return userEntity.MapToUserDto();
    }

    public async Task<UserDto> GetUserByNameAsync(string name)
    {
        throw new NotImplementedException();
    }

    public async Task<UserDto> Login(VerifyUserDto user)
    {
        var result= await _databaseContext.Users.FirstOrDefaultAsync(u=>u.Name == user.Name);
        if (result == null) throw new NotFound("The user does not exist");
        if(!BCrypt.Net.BCrypt.Verify(user.Password, result.PasswordHash)) throw new InvalidCredentialsException("Invalid password");
        return result.MapToUserDto();
    }
}