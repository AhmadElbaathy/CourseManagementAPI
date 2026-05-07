using Microsoft.AspNetCore.Mvc;
using CourseManagementAPI.DTOs;
using CourseManagementAPI.Services;

namespace CourseManagementAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Login with username and password
    /// </summary>
    /// <param name="dto">Login credentials</param>
    /// <returns>JWT token, refresh token and user info</returns>
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginDto dto)
    {
        var result = await _authService.LoginAsync(dto);
        
        if (result == null)
        {
            return Unauthorized(new { message = "Invalid username or password" });
        }

        return Ok(result);
    }

    /// <summary>
    /// Register a new user
    /// </summary>
    /// <param name="dto">User registration data</param>
    /// <returns>Created user info</returns>
    [HttpPost("register")]
    [ProducesResponseType(typeof(UserReadDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UserReadDto>> Register([FromBody] UserCreateDto dto)
    {
        var result = await _authService.RegisterAsync(dto);
        
        if (result == null)
        {
            return Conflict(new { message = "Username or email already exists" });
        }

        return Ok(result);
    }

    /// <summary>
    /// Refresh JWT token using refresh token
    /// </summary>
    /// <param name="dto">Refresh token</param>
    /// <returns>New JWT token and refresh token</returns>
    [HttpPost("refresh-token")]
    [ProducesResponseType(typeof(TokenResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<TokenResponseDto>> RefreshToken([FromBody] RefreshTokenRequestDto dto)
    {
        var result = await _authService.RefreshTokenAsync(dto.RefreshToken);
        
        if (result == null)
        {
            return Unauthorized(new { message = "Invalid or expired refresh token" });
        }

        return Ok(result);
    }

    /// <summary>
    /// Revoke a refresh token (logout)
    /// </summary>
    /// <param name="dto">Refresh token to revoke</param>
    /// <returns>Success message</returns>
    [HttpPost("revoke-token")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RevokeToken([FromBody] RefreshTokenRequestDto dto)
    {
        var result = await _authService.RevokeTokenAsync(dto.RefreshToken);
        
        if (!result)
        {
            return BadRequest(new { message = "Invalid refresh token" });
        }

        return Ok(new { message = "Token revoked successfully" });
    }
}
