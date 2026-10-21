using InfigoHR.Authentication.Interfaces;
using InfigoHR.Authentication.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace InfigoHR.Presentation.Controllers;

[ApiController]
[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
[Authorize]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [AllowAnonymous]
    [HttpPost("sign-in")]
    [EnableRateLimiting("two-factor")]
    public async Task<ActionResult<AuthResponse>> SignIn(
        SignInRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _authService.SignInAsync(
            request,
            cancellationToken);

        if (response is null)
        {
            return Unauthorized();
        }

        return Ok(response);
    }

    [AllowAnonymous]
    [HttpPost("sign-up")]
    public async Task<IActionResult> SignUp(
        SignUpRequest request,
        CancellationToken cancellationToken)
    {
        await _authService.SignUpAsync(request, cancellationToken);

        return Created();
    }
}