using GraphTaskTrackerBackend.Application.DTO;

namespace GraphTaskTrackerBackend.Application.Services.Abstractions;

public interface IUserService
{
    public Task<UserDto> CreateUserAsync(VerifyUserDto user);
    public Task<UserDto> GetUserByIdAsync(Guid id);
    public Task<UserDto> Login(VerifyUserDto user);
    public Task<ICollection<UserDto>> GetPaginatedListOfUserDtosAsync(int pageNumber, int pageSize, string? keyWordForSearch);
}