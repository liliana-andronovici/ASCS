using System.IdentityModel.Tokens.Jwt;
using InfigoHR.Authentication.Exceptions;
using InfigoHR.Authentication.Interfaces;
using InfigoHR.Authentication.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace InfigoHR.Presentation.Controllers;

[ApiController]
[Authorize]
[Route("api/auth/two-factor")]
[EnableRateLimiting("two-factor")]
[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
public class TwoFactorController : ControllerBase
{
    private readonly ITwoFactorService _service;

    public TwoFactorController(ITwoFactorService service)
    {
        _service = service;
    }

    [HttpGet("status")]
    public async Task<ActionResult<TwoFactorStatusResponse>> Status(
        CancellationToken cancellationToken)
    {
        var (employeeId, tokenVersion) = CurrentIdentity();
        var response = await _service.GetStatusAsync(
            employeeId,
            tokenVersion,
            cancellationToken);

        return Ok(response);
    }

    [HttpPost("setup")]
    public async Task<ActionResult<TwoFactorSetupResponse>> Setup(
        TwoFactorSetupRequest request,
        CancellationToken cancellationToken)
    {
        var (employeeId, tokenVersion) = CurrentIdentity();
        var response = await _service.SetupAsync(
            employeeId,
            tokenVersion,
            request,
            cancellationToken);

        return Ok(response);
    }

    [HttpPost("enable")]
    public async Task<ActionResult<TwoFactorRecoveryResponse>> Enable(
        TwoFactorEnableRequest request,
        CancellationToken cancellationToken)
    {
        var (employeeId, tokenVersion) = CurrentIdentity();
        var response = await _service.EnableAsync(
            employeeId,
            tokenVersion,
            request,
            cancellationToken);

        return Ok(response);
    }

    [AllowAnonymous]
    [HttpPost("verify")]
    public async Task<ActionResult<AuthResponse>> Verify(
        TwoFactorChallengeRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _service.VerifyAsync(
            request,
            cancellationToken);

        return Ok(response);
    }

    [HttpPost("recovery-codes/regenerate")]
    public async Task<ActionResult<TwoFactorRecoveryResponse>> Regenerate(
        TwoFactorManagementRequest request,
        CancellationToken cancellationToken)
    {
        var (employeeId, tokenVersion) = CurrentIdentity();
        var response = await _service.RegenerateAsync(
            employeeId,
            tokenVersion,
            request,
            cancellationToken);

        return Ok(response);
    }

    [HttpPost("disable")]
    public async Task<ActionResult<AuthResponse>> Disable(
        TwoFactorManagementRequest request,
        CancellationToken cancellationToken)
    {
        var (employeeId, tokenVersion) = CurrentIdentity();
        var response = await _service.DisableAsync(
            employeeId,
            tokenVersion,
            request,
            cancellationToken);

        return Ok(response);
    }

    private (int EmployeeId, Guid TokenVersion) CurrentIdentity()
    {
        var validId = int.TryParse(User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value, out var id);
        var validVersion = Guid.TryParse(User.FindFirst("token_version")?.Value, out var version);
        if (!validId || !validVersion)
            throw TwoFactorException.Invalid();
        return (id, version);
    }
}
