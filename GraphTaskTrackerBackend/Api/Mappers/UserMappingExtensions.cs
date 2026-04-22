using GraphTaskTrackerBackend.Api.Models;
using GraphTaskTrackerBackend.Application.DTO;
using Riok.Mapperly.Abstractions;

namespace GraphTaskTrackerBackend.Api.Mappers;

[Mapper]
public static partial class UserMappingExtensions
{
    public static partial VerifyUserDto MapToVerifyUserDto(this VerifyUserRequest registration);
    public static partial UserMessage MapToUserMessage(this UserDto userDtos);
    public static partial ICollection<UserMessage> MapToListOfUserMessages(this IEnumerable<UserDto> userDtos);
    public static partial ProfileMessage MapTOProfileMessage(this UserDto userDto);
}