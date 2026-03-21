using GraphTaskTrackerBackend.Application.DTO;

namespace GraphTaskTrackerBackend.Application.Services.Abstractions;

public interface IUserService
{
    public Task<UserDto> CreateUserAsync(VerifyUserDto user);
    public Task<UserDto> GetUserByNameAsync(string name);
    public Task<UserDto> Login(VerifyUserDto user);
}