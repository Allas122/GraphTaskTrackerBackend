using GraphTaskTrackerBackend.Api.Models;
using GraphTaskTrackerBackend.Application.DTO;
using Riok.Mapperly.Abstractions;

namespace GraphTaskTrackerBackend.Api.Mappers;

[Mapper]
public static partial class UserMappingExtensions
{
    public static partial VerifyUserDto MapToVerifyUserDto(this VerifyUserRequest registration);
}