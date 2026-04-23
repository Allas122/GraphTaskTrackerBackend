using EFCore.BulkExtensions;
using GraphTaskTrackerBackend.Application.DTO;
using GraphTaskTrackerBackend.Application.Exceptions;
using GraphTaskTrackerBackend.Application.Mappers;
using GraphTaskTrackerBackend.Application.Services.Abstractions;
using GraphTaskTrackerBackend.Domain.Entities;
using GraphTaskTrackerBackend.Infrastructure.DataBase;
using GraphTaskTrackerBackend.Infrastructure.Events.Abstractions;
using GraphTaskTrackerBackend.Infrastructure.Events.Implementations.Messages;
using Microsoft.EntityFrameworkCore;

namespace GraphTaskTrackerBackend.Application.Services.Implementations;

public class GraphService : IGraphService
{
    private readonly DatabaseContext _databaseContext;
    private readonly IEventController<Guid, GraphEvent> _graphEventService;
    private readonly ILogger<GraphService> _logger;


    public GraphService(
        DatabaseContext databaseContext,
        ILogger<GraphService> logger,
        IEventController<Guid, GraphEvent> graphEventService)
    {
        _databaseContext = databaseContext;
        _logger = logger;
        _graphEventService = graphEventService;
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

    public async Task SynchronizeGraphAsync(SyncGraphDto dto, Guid userId)
    {
        var graph = await _databaseContext.Graphs.AsNoTracking().FirstOrDefaultAsync(g => g.Id == dto.GraphId)
                    ?? throw new NotFound("Graph not found");

        var incomingIds = dto.Nodes.Select(n => n.Id).ToList();
        if (await _databaseContext.Nodes.AnyAsync(n => incomingIds.Contains(n.Id) && n.GraphId != dto.GraphId))
            throw new Conflict("Node ID conflict");

        await ExecuteBulkSynchronizationAsync(dto, userId, incomingIds);
        await _graphEventService.PostAsync(dto.GraphId,
            new GraphEvent { GraphId = dto.GraphId, EventType = GraphEventType.Update });
    }

    public async Task<GraphDto> GetGraphDtoByIdAsync(Guid id)
    {
        return (await _databaseContext.Graphs.FirstOrDefaultAsync()).MapToGraphDto();
    }

    public async Task<SyncGraphDto> GetSyncGraphDtoByGraphIdAsync(Guid id)
    {
        var graphInfo = await _databaseContext.Graphs
            .AsNoTracking()
            .Where(g => g.Id == id)
            .Select(g => new { g.Id, g.UserId })
            .FirstOrDefaultAsync();

        if (graphInfo == null) throw new NotFound("Graph not found");
        
        var nodes = await _databaseContext.Nodes
            .AsNoTracking()
            .Include(n => n.Assigned) 
            .Where(n => n.GraphId == id)
            .ToListAsync();
        
        var nodeIds = nodes.Select(n => n.Id).ToList();
        var edges = await _databaseContext.Edges
            .AsNoTracking()
            .Where(e => nodeIds.Contains(e.FromNodeId) || nodeIds.Contains(e.ToNodeId)) 
            .ToListAsync();
        
        return new SyncGraphDto
        {
            GraphId = id,
            UserId = graphInfo.UserId,
            Nodes = nodes.MapToListOfNodeDtos(),
            Edges = edges.MapToListOfEdgeDtos()
        };
    }

    public async Task AddAssignedUserAsync(Guid userId, Guid nodeId)
    {
        if (!_databaseContext.Users.Any(u => u.Id == userId)) throw new Unprocessable("User not found");
        if (!_databaseContext.Nodes.Any(n => n.Id == nodeId)) throw new Unprocessable("Node not found");
        if (_databaseContext.AssignedUsers.Any(n => n.NodeId == nodeId && n.UserId == userId))
            throw new AlreadyExistsException("Already assigned");
        var assigned = new AssignedUser
        {
            UserId = userId,
            NodeId = nodeId
        };
        await _databaseContext.AssignedUsers.AddAsync(assigned);
    }

    public async Task RemoveAssignedUserAsync(Guid userId, Guid nodeId)
    {
        await _databaseContext.Set<AssignedUser>()
            .Where(a => a.UserId == userId && a.NodeId == nodeId)
            .ExecuteDeleteAsync();
    }


    public async Task<ICollection<GraphWithoutNodesDto>> GetPaginatedListOfGraphDtosAsync(int pageNumber, int pageSize,
        string? keyWordForSearch)
    {
        IQueryable<Graph> query = _databaseContext.Graphs;

        if (!string.IsNullOrWhiteSpace(keyWordForSearch))
        {
            var searchPattern = $"%{keyWordForSearch}%";
            query = query.Where(n => EF.Functions.ILike(n.Name, searchPattern));
        }

        var graphs = await query
            .OrderBy(n => n.CreatedAt)
            .ThenBy(n => n.Id)
            .Skip(pageSize * (pageNumber - 1))
            .Take(pageSize)
            .ToListAsync();

        return graphs.MapToListOfGraphWithoutNodesDtos();
    }

    private async Task ExecuteBulkSynchronizationAsync(SyncGraphDto dto, Guid userId, List<Guid> incomingNodeIds)
    {
        await _databaseContext.Database.CreateExecutionStrategy().ExecuteAsync(async () =>
        {
            using var tx = await _databaseContext.Database.BeginTransactionAsync();
            try
            {
                var incomingNodes = dto.Nodes.MapToListOfNodes().ToDictionary(n => n.Id, n => n);
                var existingNodes = await _databaseContext.Nodes
                    .Include(n => n.Assigned) 
                    .Where(n => incomingNodeIds.Contains(n.Id)).ToListAsync();

                var conflictNodesIds = await _databaseContext.Nodes
                    .Where(n => incomingNodeIds.Contains(n.Id) && n.GraphId != dto.GraphId)
                    .Select(n=>n.Id).ToListAsync();
                if(conflictNodesIds.Count > 0)
                    throw new Conflict($"Conflict node IDs : [{string.Join(", ",conflictNodesIds)}]");
                var existingIdsSet = existingNodes.Select(n => n.Id).ToHashSet();
                
                var assignedUserIds = dto.Nodes
                    .SelectMany(n => n.Assigned)
                    .Select(a => a.Id)
                    .Distinct()
                    .ToArray();
                
                var assignedUsersDict = await _databaseContext.Users
                    .Where(u => assignedUserIds.Contains(u.Id))
                    .ToDictionaryAsync(u => u.Id);
                
                if (assignedUserIds.Length != assignedUsersDict.Count)
                {
                    var missingIds = assignedUserIds
                        .Where(id => !assignedUsersDict.ContainsKey(id))
                        .ToList();
                    
                    throw new Unprocessable($"User id's [{string.Join(", ", missingIds)}] doesn't exist");
                }
                var nodeAssigned = dto.Nodes.ToDictionary(n => n.Id, n => n.Assigned.Select(u => u.Id).ToList());
                foreach (var existingNode in existingNodes)
                {
                    var incomingNode = incomingNodes[existingNode.Id];
                    existingNode.Name = incomingNode.Name;
                    existingNode.Description = incomingNode.Description;
                    existingNode.Time =  incomingNode.Time;
                    var users = nodeAssigned[existingNode.Id]
                        .Select(id=>assignedUsersDict[id])
                        .ToList();
                    existingNode.Assigned = users;
                }
                await _databaseContext.Nodes
                    .Where(n => n.GraphId == dto.GraphId && !incomingNodeIds.Contains(n.Id))
                    .ExecuteDeleteAsync();
                
                var newNodes = incomingNodes.Values
                    .Where(n => !existingIdsSet.Contains(n.Id))
                    .ToList();
                
                foreach (var newNode in newNodes)
                {
                    var incomingNode = incomingNodes[newNode.Id];
                    newNode.GraphId = dto.GraphId;
                    newNode.CreatedAt = DateTime.UtcNow;
                    newNode.AuthorId = userId;
                    newNode.Assigned = nodeAssigned[newNode.Id]
                        .Select(id => assignedUsersDict[id])
                        .ToList();
                }
                await _databaseContext.Nodes.AddRangeAsync(newNodes);
                
                var newEdges = dto.Edges.Select(e => new Edge
                {
                    FromNodeId = e.FromNodeId,
                    ToNodeId = e.ToNodeId,
                    Weight = e.Weight
                });
                
                await _databaseContext.Edges
                    .Where(e => _databaseContext.Nodes
                        .Where(n => n.GraphId == dto.GraphId)
                        .Select(n => n.Id).Contains(e.FromNodeId))
                    .ExecuteDeleteAsync();
                
                await _databaseContext.Edges.AddRangeAsync(newEdges);
                await _databaseContext.SaveChangesAsync();
                await tx.CommitAsync();
            }
            catch (Exception)
            {
                await tx.RollbackAsync();
                throw;
            }
        });
    }

    private async Task<ICollection<Guid>> GetUnexistingUsers(ICollection<Guid> assignedUserIds)
    {
        var existingIds =
            await _databaseContext.Users
                .Where(u => assignedUserIds.Contains(u.Id))
                .Select(n => Guid.Parse(n.Name)).ToListAsync();
        var unexistingIds = assignedUserIds.Except(existingIds).ToArray();
        return unexistingIds;
    }
}