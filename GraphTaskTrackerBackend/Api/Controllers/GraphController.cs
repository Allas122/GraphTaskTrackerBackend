using System.Security.Claims;
using FluentValidation;
using GraphTaskTrackerBackend.Api.Mappers;
using GraphTaskTrackerBackend.Api.Models;
using GraphTaskTrackerBackend.Application.DTO;
using GraphTaskTrackerBackend.Application.Mappers;
using GraphTaskTrackerBackend.Application.Services.Abstractions;
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
    private IGraphService _graphService;
    public GraphController(
        IGraphService graphService
        )
    {
        _graphService = graphService;
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
    [HttpPut("/sync")]
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
        await _graphService.SynchronizeGraphAsync(dto);
        return "Your graph has been synced";
    }
}