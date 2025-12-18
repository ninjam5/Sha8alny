using Microsoft.AspNetCore.Mvc;
using Sh8lny.Abstraction.Services;
using Sh8lny.Shared.DTOs.Auth;

namespace Sh8lny.Web.Controllers;

/// <summary>
/// Controller for authentication endpoints (register and login).
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Registers a new user.
    /// </summary>
    /// <param name="dto">Registration data containing email, password, and role.</param>
    /// <returns>Authentication response with JWT token if successful.</returns>
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterDto dto)
    {
        var result = await _authService.RegisterAsync(dto);

        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Authenticates a user and returns a JWT token.
    /// </summary>
    /// <param name="dto">Login credentials containing email and password.</param>
    /// <returns>Authentication response with JWT token if successful.</returns>
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto dto)
    {
        var result = await _authService.LoginAsync(dto);

        if (!result.IsSuccess)
        {
            return Unauthorized(result);
        }

        return Ok(result);
    }
}
