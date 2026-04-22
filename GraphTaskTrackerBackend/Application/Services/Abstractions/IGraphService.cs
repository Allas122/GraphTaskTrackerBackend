using GraphTaskTrackerBackend.Application.DTO;

namespace GraphTaskTrackerBackend.Application.Services.Abstractions;

public interface IGraphService
{
    public Task<GraphWithoutNodesDto> CreateGraphWithoutNodesAsync(CreateGraphDto syncGraphDto);
    public Task SynchronizeGraphAsync(SyncGraphDto syncGraphDto, Guid userId);
    public Task<GraphDto> GetGraphDtoByIdAsync(Guid id);
    public Task<SyncGraphDto> GetSyncGraphDtoByGraphIdAsync(Guid id);
    public Task AddAssignedUserAsync(Guid userId, Guid nodeId);
    public Task RemoveAssignedUserAsync(Guid userId, Guid nodeId);
    public Task<ICollection<GraphWithoutNodesDto>> GetPaginatedListOfGraphDtosAsync(int pageNumber, int pageSize, string? keyWordForSearch);
}