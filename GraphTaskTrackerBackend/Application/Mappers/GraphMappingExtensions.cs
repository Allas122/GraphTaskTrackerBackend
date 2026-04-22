using GraphTaskTrackerBackend.Api.Models;
using GraphTaskTrackerBackend.Application.DTO;
using GraphTaskTrackerBackend.Domain.Entities;
using Riok.Mapperly.Abstractions;

namespace GraphTaskTrackerBackend.Application.Mappers;

[Mapper]
public static partial class GraphMappingExtensions
{
    public static partial Graph MapToGraph(this GraphDto graphDto);
    public static partial Graph MapToGraph(this CreateGraphDto syncGraph);
    public static partial GraphWithoutNodesDto MapToGraphWithoutNodesDto(this GraphDto graphDto);
    public static partial GraphWithoutNodesDto MapToGraphWithoutNodesDto(this Graph graph);
    public static partial GraphDto MapToGraphDto(this Graph graph);
    public static partial IEnumerable<Node> MapToListOfNodes(this IEnumerable<NodeDto> nodeDtos);
    public static partial IEnumerable<Edge> MapToListOfEdges(this IEnumerable<EdgeDto> edgeDtos);
    public static partial NodeDto MapToNodeDto(this Node node);
    public static partial ICollection<NodeDto> MapToListOfNodeDtos(this IEnumerable<Node> graphDtos);
    public static partial ICollection<EdgeDto> MapToListOfEdgeDtos(this IEnumerable<Edge> edgeDtos);
    public static partial SyncGraphResponse MapToSyncGraphResponse(this SyncGraphDto graphDto);
    public static partial ICollection<GraphDto> MapToListOfGraphDtos(this IEnumerable<Graph> graphDtos);
    public static partial ICollection<GraphWithoutNodesDto> MapToListOfGraphWithoutNodesDtos(this IEnumerable<Graph> graphDtos);
}