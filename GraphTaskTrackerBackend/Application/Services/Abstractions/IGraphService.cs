using GraphTaskTrackerBackend.Application.DTO;

namespace GraphTaskTrackerBackend.Application.Services.Abstractions;

public interface IGraphService
{
    public Task<GraphWithoutNodesDto> CreateGraphWithoutNodesAsync(CreateGraphDto createGraphDto);
}