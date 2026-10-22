using InfigoHR.Authentication.Interfaces;
using InfigoHR.Authentication.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace InfigoHR.Presentation.Controllers;

[ApiController]
[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
[AllowAnonymous]
[Route("api/auth/password-reset")]
[EnableRateLimiting("password-reset")]
public class PasswordResetController : ControllerBase
{
    private readonly IPasswordResetService _service;

    public PasswordResetController(IPasswordResetService service)
    {
        _service = service;
    }

    [HttpPost("request")]
    public async Task<IActionResult> RequestReset(
        RequestPasswordResetRequest request,
        CancellationToken cancellationToken)
    {
        await _service.RequestAsync(request, cancellationToken);

        return Accepted();
    }

    [HttpPost("verify")]
    public async Task<ActionResult<VerifyPasswordResetResponse>> Verify(
        VerifyPasswordResetRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _service.VerifyAsync(
            request,
            cancellationToken);

        return Ok(response);
    }

    [HttpPost("complete")]
    public async Task<ActionResult<AuthResponse>> Complete(
        CompletePasswordResetRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _service.CompleteAsync(
            request,
            cancellationToken);

        return Ok(response);
    }
}