using System.Security.Claims;
using FluentValidation;
using GraphTaskTrackerBackend.Api.Mappers;
using GraphTaskTrackerBackend.Api.Models;
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
    [HttpPost("/user/me")]
    public async Task<ActionResult<string>> Me()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Ok(userId);
    }
}