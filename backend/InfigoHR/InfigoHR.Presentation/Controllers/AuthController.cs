using System.IdentityModel.Tokens.Jwt;
using InfigoHR.Authentication.Models;
using InfigoHR.Authentication.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InfigoHR.Presentation.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [AllowAnonymous]
    [HttpPost("sign-in")]
    public async Task<ActionResult<AuthResponse>> SignIn(
        SignInRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _authService.SignInAsync(
            request,
            cancellationToken);

        if (response is null)
        {
            return Unauthorized(new
            {
                Message = "Invalid email or password."
            });
        }

        return Ok(response);
    }

    [AllowAnonymous]
    [HttpPost("sign-up")]
    public async Task<ActionResult<AuthResponse>> SignUp(
        SignUpRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _authService.SignUpAsync(
            request,
            cancellationToken);

        if (response is null)
        {
            return Conflict(new
            {
                Message = "An account with this email already exists."
            });
        }

        return StatusCode(
            StatusCodes.Status201Created,
            response);
    }

    [Authorize]
    [HttpGet("me")]
    public IActionResult Me()
    {
        return Ok(new
        {
            EmployeeId = User.FindFirst(
                JwtRegisteredClaimNames.Sub)?.Value,
            Email = User.FindFirst(
                JwtRegisteredClaimNames.Email)?.Value
        });
    }
}