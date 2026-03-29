using EFCore.BulkExtensions;
using GraphTaskTrackerBackend.Application.DTO;
using GraphTaskTrackerBackend.Application.Exceptions;
using GraphTaskTrackerBackend.Application.Mappers;
using GraphTaskTrackerBackend.Application.Services.Abstractions;
using GraphTaskTrackerBackend.Infrastructure.DataBase;
using Microsoft.EntityFrameworkCore;

namespace GraphTaskTrackerBackend.Application.Services.Implementations;

public class GraphService : IGraphService
{
    private readonly DatabaseContext _databaseContext;
    private readonly ILogger<GraphService> _logger;

    public GraphService(
        DatabaseContext databaseContext,
        ILogger<GraphService> logger)
    {
        _databaseContext = databaseContext;
        _logger = logger;
    }

    public async Task<GraphWithoutNodesDto> CreateGraphWithoutNodesAsync(CreateGraphDto syncGraphDto)
    {
        var result = syncGraphDto.MapToGraph();
        result.CreatedAt = DateTime.UtcNow;
        result.UpdatedAt = DateTime.UtcNow;
        if (!await _databaseContext.Users.AnyAsync(u => u.Id == result.UserId))
            throw new Unprocessable("The user is not existed");
        await _databaseContext.Graphs.AddAsync(result);
        await _databaseContext.SaveChangesAsync();
        return result.MapToGraphWithoutNodesDto();
    }

    public async Task SynchronizeGraphAsync(SyncGraphDto syncGraphDto)
    {
        var graph = await _databaseContext.Graphs.AsNoTracking().FirstOrDefaultAsync(g => g.Id == syncGraphDto.GraphId);
        if (graph is null) throw new NotFound("Graph does not exist");
        if (graph.UserId != syncGraphDto.UserId) throw new Forbidden("Not allowed");

        var incomingNodeIds = syncGraphDto.Nodes.Select(n => n.Id).ToList();

        var conflictingNodeIds = await _databaseContext.Nodes
            .AsNoTracking()
            .Where(n => incomingNodeIds.Contains(n.Id) && n.GraphId != syncGraphDto.GraphId)
            .Select(n => n.Id)
            .ToListAsync();
        
        if (conflictingNodeIds.Any())
            throw new Conflict($"Node IDs already used in another graph: {string.Join(", ", conflictingNodeIds)}");

        foreach (var node in syncGraphDto.Nodes)
        {
            node.GraphId = syncGraphDto.GraphId;
        }
        await ExecuteBulkSynchronizationAsync(syncGraphDto);
    }

    private async Task ExecuteBulkSynchronizationAsync(SyncGraphDto dto)
    {
        var strategy = _databaseContext.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        {
            using var transaction = await _databaseContext.Database.BeginTransactionAsync();
            try
            {
                var nodesToSync = dto.Nodes.MapToListOfNodes();
                foreach (var node in nodesToSync) node.GraphId = dto.GraphId;

                var edgesToSync = dto.Edges.MapToListOfEdges();
                var incomingNodeIds = nodesToSync.Select(n => n.Id).ToList();

                await _databaseContext.Edges
                    .Where(e => _databaseContext.Nodes.Any(n =>
                        n.GraphId == dto.GraphId && (n.Id == e.FromNodeId || n.Id == e.ToNodeId)))
                    .ExecuteDeleteAsync();

                await _databaseContext.BulkInsertOrUpdateAsync(nodesToSync);

                await _databaseContext.Nodes
                    .Where(n => n.GraphId == dto.GraphId && !incomingNodeIds.Contains(n.Id))
                    .ExecuteDeleteAsync();

                if (edgesToSync.Any()) await _databaseContext.BulkInsertAsync(edgesToSync);

                await transaction.CommitAsync();
                _logger.LogInformation("Successfully synchronized graph {GraphId} (Postgres Compatible)", dto.GraphId);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Postgres Bulk Synchronization failed for graph {GraphId}", dto.GraphId);
                throw;
            }
        });
    }
}