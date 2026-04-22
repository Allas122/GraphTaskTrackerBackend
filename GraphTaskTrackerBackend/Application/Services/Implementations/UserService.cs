using GraphTaskTrackerBackend.Application.DTO;
using GraphTaskTrackerBackend.Application.Exceptions;
using GraphTaskTrackerBackend.Application.Mappers;
using GraphTaskTrackerBackend.Application.Services.Abstractions;
using GraphTaskTrackerBackend.Domain.Entities;
using GraphTaskTrackerBackend.Infrastructure.DataBase;
using GraphTaskTrackerBackend.Infrastructure.Events.Abstractions;
using GraphTaskTrackerBackend.Infrastructure.Events.Implementations.Messages;
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

    public async Task<UserDto> GetUserByIdAsync(Guid id)
    {
        var userEntity = await _databaseContext.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (userEntity == null)
        {
            throw new NotFound("The user does not exist");
        }
        return userEntity.MapToUserDto();
    }

    public async Task<UserDto> Login(VerifyUserDto user)
    {
        var result= await _databaseContext.Users.FirstOrDefaultAsync(u=>u.Name == user.Name);
        if (result == null) throw new NotFound("The user does not exist");
        if(!BCrypt.Net.BCrypt.Verify(user.Password, result.PasswordHash)) throw new InvalidCredentialsException("Invalid password");
        return result.MapToUserDto();
    }

    public async Task<ICollection<UserDto>> GetPaginatedListOfUserDtosAsync(int pageNumber, int pageSize, string? keyWordForSearch)
    {
        IQueryable<User> query = _databaseContext.Users;

        if (!string.IsNullOrWhiteSpace(keyWordForSearch))
        {
            var searchPattern = $"%{keyWordForSearch}%";
            query = query.Where(n => EF.Functions.ILike(n.Name, searchPattern));
        }

        var users = await query
            .OrderBy(n => n.Name)
            .ThenBy(n => n.Id)
            .Skip(pageSize * (pageNumber - 1))
            .Take(pageSize)
            .ToListAsync();

        return users.MapToListOfUserDtos();
    }
}