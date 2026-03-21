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
    public async Task<GraphWithoutNodesDto> CreateGraphWithoutNodesAsync(CreateGraphDto createGraphDto)
    {
        var result = createGraphDto.MapToGraph();
        result.CreatedAt = DateTime.UtcNow;
        result.UpdatedAt = DateTime.UtcNow;
        if (!await _databaseContext.Users.AnyAsync(u=>u.Id==result.UserId)) throw new Unprocessable("The user is not existed");
        await _databaseContext.Graphs.AddAsync(result);
        await  _databaseContext.SaveChangesAsync();
        return result.MapToGraphWithoutNodesDto();
    }
}