using GraphTaskTrackerBackend.Application.DTO;
using GraphTaskTrackerBackend.Domain.Entities;
using Riok.Mapperly.Abstractions;

namespace GraphTaskTrackerBackend.Application.Mappers;

[Mapper]
public static partial class UserMappingExtensions
{
    public static partial UserDto MapToUserDto(this User user);
    public static partial ICollection<UserDto> MapToListOfUserDtos(this IEnumerable<User> users);
}