using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;
using FluentValidation;
using GraphTaskTrackerBackend.Api.Mappers;
using GraphTaskTrackerBackend.Api.Models;
using GraphTaskTrackerBackend.Application.DTO;
using GraphTaskTrackerBackend.Application.Mappers;
using GraphTaskTrackerBackend.Application.Services.Abstractions;
using GraphTaskTrackerBackend.Infrastructure.Events.Abstractions;
using GraphTaskTrackerBackend.Infrastructure.Events.Implementations.Messages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GraphTaskTrackerBackend.Api.Controllers;

[ApiController]
[Route("/graph")]
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status422UnprocessableEntity)]
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
[ProducesResponseType(typeof(ValidationErrorResponse), StatusCodes.Status400BadRequest)]
public class GraphController : ControllerBase
{
    private readonly IGraphService _graphService;
    private readonly IEventController<Guid, GraphEvent> _graphEventController;
    
    public GraphController(
        IGraphService graphService,
        IEventController<Guid, GraphEvent> graphEventService
        )
    {
        _graphService = graphService;
        _graphEventController = graphEventService;
    }

    [Authorize]
    [HttpPost("/create")]
    [ProducesResponseType(typeof(CreateGraphResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<CreateGraphResponse>> CreateGraph(
        CreateGraphRequest response,
        [FromServices] IValidator<CreateGraphRequest> validator)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null) throw new UnauthorizedAccessException("Unauthorized");
        validator.ValidateAndThrow(response);
        var dto = response.MapToCreateGraphDto();
        dto.UserId = Guid.Parse(userId);
        var result = await _graphService.CreateGraphWithoutNodesAsync(dto);
        return Ok(result.MapToCreateGraphResponse());
    }

    [Authorize]
    [HttpPatch("/sync")]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<string> SyncGraph(
        SyncGraphRequest request,
        [FromServices] IValidator<SyncGraphRequest> validator)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null) throw new UnauthorizedAccessException("Unauthorized");
        validator.ValidateAndThrow(request);
        var dto = request.MapToSyncGraphDto();
        dto.UserId = Guid.Parse(userId);
        await _graphService.SynchronizeGraphAsync(dto, Guid.Parse(userId));
        return "Your graph has been synced";
    }

    private async Task NotifySSE(CancellationToken ct, Guid graphId)
    {
        var state = await _graphService.GetSyncGraphDtoByGraphIdAsync(graphId);
        var json = JsonSerializer.Serialize(state.MapToSyncGraphResponse());
        await Response.WriteAsync($"data:{json}\n\n");
        await Response.Body.FlushAsync(ct);
    }
    
    [HttpGet("/{graphId}")]
    [Authorize]
    [ProducesResponseType(typeof(SyncGraphResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<SyncGraphResponse>> Get([FromRoute] Guid graphId, CancellationToken ct)
    {
        return (await _graphService.GetSyncGraphDtoByGraphIdAsync(graphId)).MapToSyncGraphResponse();
    }
    
    [HttpGet("/{graphId}/sse")]
    [Authorize]
    [ProducesResponseType(typeof(SyncGraphResponse), StatusCodes.Status200OK)]
    public async Task GetSse([FromRoute] Guid graphId, CancellationToken ct)
    {
        Response.ContentType = "text/event-stream";
        Response.Headers.Add("Cache-Control", "no-cache");
        Response.Headers.Add("X-Accel-Buffering", "no");
        
        await NotifySSE(ct, graphId);
        
        var eventStream = _graphEventController.SubscribeAsync(graphId,ct);
        await foreach (var ev in eventStream.WithCancellation(ct))
        {
            await NotifySSE(ct, graphId);
        }
    }

    [HttpPost("/make-assigned")]
    [Authorize]
    public async Task<string> MakeAssigned([FromBody] MakeAssignedRequest req)
    {
        await _graphService.AddAssignedUserAsync(req.UserId, req.NodeId);
        return "Ok!";
    }
    [HttpPost("/remove-assigned")]
    [Authorize]
    public async Task<string> RemoveAssigned([FromBody] MakeAssignedRequest req)
    {
        await _graphService.RemoveAssignedUserAsync(req.UserId, req.NodeId);
        return "Ok!";
    }

    
    [Authorize]
    [HttpGet("/get-paginated-graph")]
    [ProducesResponseType(typeof(ICollection<GraphCartResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ICollection<GraphCartResponse>>> GetPaginatedGraph(
        [FromQuery] PaginationQuery query,
        [FromServices] IValidator<PaginationQuery> validator)
    {
        await validator.ValidateAndThrowAsync(query);
        var a = await _graphService.GetPaginatedListOfGraphDtosAsync(query.PageNumber, query.PageSize, query.KeyWordForSearch);
        return Ok(a.MapToListOfGraphCartResponses());
    }

    [HttpDelete("/{graphId}")]
    [Authorize]
    public async Task DeleteGraph([FromRoute] Guid graphId)
    {
        await _graphService.DeleteGraphByIdAsync(graphId);
    }
}