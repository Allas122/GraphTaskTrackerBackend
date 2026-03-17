using GraphTaskTrackerBackend.Application.DTO;

namespace GraphTaskTrackerBackend.Application.Services.Abstractions;

public interface IUserService
{
    public Task<UserDTO> CreateUserAsync(VerifyUserDto user);
    public Task<UserDTO> GetUserByNameAsync(string name);
    public Task<UserDTO> Login(VerifyUserDto user);
}