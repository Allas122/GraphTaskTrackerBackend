using GraphTaskTrackerBackend.Api.Models;
using GraphTaskTrackerBackend.Application.DTO;
using GraphTaskTrackerBackend.Domain.Entities;
using Riok.Mapperly.Abstractions;

namespace GraphTaskTrackerBackend.Application.Mappers;

[Mapper]
public static partial class GraphMappingExtensions
{
    public static partial Graph MapToGraph(this GraphDto graphDto);
    public static partial Graph MapToGraph(this CreateGraphDto createGraph);
    public static partial GraphWithoutNodesDto MapToGraphWithoutNodesDto(this GraphDto graphDto);
    public static partial GraphWithoutNodesDto MapToGraphWithoutNodesDto(this Graph graph);
    public static partial GraphDto MapToGraphDto(this Graph graph);
}