using GraphTaskTrackerBackend.Application.DTO;
using GraphTaskTrackerBackend.Domain.Entities;
using Riok.Mapperly.Abstractions;

namespace GraphTaskTrackerBackend.Application.Mappers;

[Mapper]
public static partial class UserMappingExtensions
{
    public static partial UserDTO MapToUserDto(this User user);
}