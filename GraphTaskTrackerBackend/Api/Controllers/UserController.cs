using System.Security.Claims;
using FluentValidation;
using GraphTaskTrackerBackend.Api.Mappers;
using GraphTaskTrackerBackend.Api.Models;
using GraphTaskTrackerBackend.Application.DTO;
using GraphTaskTrackerBackend.Application.Services.Abstractions;
using GraphTaskTrackerBackend.Infrastructure.Security.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GraphTaskTrackerBackend.Api.Controllers;

[ApiController]
[Route("/user")]
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
[ProducesResponseType(typeof(ValidationErrorResponse), StatusCodes.Status400BadRequest)]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;
    private readonly IUserService _userService;
    private readonly IJwtService _jwtService;
    private readonly IValidator<VerifyUserRequest> _userValidator;
    
    public UserController(
        ILogger<UserController> logger,
        IUserService userService,
        IJwtService jwtService,
        IValidator<VerifyUserRequest>  userValidator
        )
    {
        _logger = logger;
        _userService = userService;
        _jwtService = jwtService;
        _userValidator = userValidator;
    }

    [HttpPost("/registration")]
    [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<TokenResponse>> RegisterAsync(
        VerifyUserRequest request)
    {
        await _userValidator.ValidateAndThrowAsync(request);
        
        var user=await _userService.CreateUserAsync(request.MapToVerifyUserDto());
        return Ok(new
        {
            Token = _jwtService.GenerateToken(user.Id)
        });
    }
    [HttpPost("/login")]
    [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<TokenResponse>> LoginAsync(VerifyUserRequest request)
    {
        await _userValidator.ValidateAndThrowAsync(request);
        var user = await _userService.Login(request.MapToVerifyUserDto());
        return Ok(new
        {
            Token = _jwtService.GenerateToken(user.Id)
        });
    }
    
    [Authorize]
    [HttpGet("/user/me")]
    public async Task<ActionResult<ProfileMessage>> Me()
    {
        var userId =Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        var user= await _userService.GetUserByIdAsync(userId);
        return Ok(user.MapTOProfileMessage());
    }

    [Authorize]
    [HttpGet("/user/list")]
    public async Task<ActionResult<List<UserMessage>>> GetUsers(
        [FromQuery] PaginationQuery pq,
        [FromServices] IValidator<PaginationQuery> validator)
    {
        await validator.ValidateAndThrowAsync(pq);
        var users = 
            (await _userService.GetPaginatedListOfUserDtosAsync(pq.PageNumber,pq.PageSize,pq.KeyWordForSearch))
            .MapToListOfUserMessages();
        return Ok(users);
    }
}