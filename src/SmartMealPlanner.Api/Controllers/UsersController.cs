using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartMealPlanner.Application.Common.Interfaces;
using SmartMealPlanner.Application.Common.Models;
using SmartMealPlanner.Application.Users.DTOs;
using SmartMealPlanner.Application.Users.Interfaces;
using SmartMealPlanner.Domain.Exceptions;

namespace SmartMealPlanner.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly IUserProfileService _userProfileService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IValidator<UpdateUserProfileRequest> _updateValidator;

    public UsersController(
        IUserProfileService userProfileService,
        ICurrentUserService currentUserService,
        IValidator<UpdateUserProfileRequest> updateValidator)
    {
        _userProfileService = userProfileService;
        _currentUserService = currentUserService;
        _updateValidator = updateValidator;
    }

    [HttpGet("me")]
    [ProducesResponseType(typeof(ApiResult<UserProfileDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResult<UserProfileDto>>> GetMe(CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var profile = await _userProfileService.GetMyProfileAsync(userId, cancellationToken);
        return Ok(ApiResult<UserProfileDto>.Ok(profile));
    }

    [HttpPut("me")]
    [ProducesResponseType(typeof(ApiResult<UserProfileDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResult<UserProfileDto>>> UpdateMe(
        [FromBody] UpdateUserProfileRequest request,
        CancellationToken cancellationToken)
    {
        await _updateValidator.ValidateAndThrowAsync(request, cancellationToken);
        var userId = GetCurrentUserId();
        var profile = await _userProfileService.UpdateMyProfileAsync(userId, request, cancellationToken);
        return Ok(ApiResult<UserProfileDto>.Ok(profile));
    }

    private Guid GetCurrentUserId() =>
        _currentUserService.UserId
        ?? throw new UnauthorizedException("User is not authenticated.");
}
