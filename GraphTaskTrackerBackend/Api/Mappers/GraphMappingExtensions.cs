using GraphTaskTrackerBackend.Api.Models;
using GraphTaskTrackerBackend.Application.DTO;
using Riok.Mapperly.Abstractions;

namespace GraphTaskTrackerBackend.Api.Mappers;

[Mapper]
public static partial class GraphMappingExtensions
{
    public static partial CreateGraphDto MapToCreateGraphDto(this CreateGraphRequest createGraph);
    public static partial CreateGraphResponse MapToCreateGraphResponse(this GraphWithoutNodesDto graph);
    public static partial SyncGraphDto MapToSyncGraphDto(this SyncGraphRequest syncGraph);
    
}