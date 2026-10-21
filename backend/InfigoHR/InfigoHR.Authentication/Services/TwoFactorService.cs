using FluentValidation;
using InfigoHR.Application.Interfaces.Repositories;
using InfigoHR.Authentication.Exceptions;
using InfigoHR.Authentication.Interfaces;
using InfigoHR.Authentication.Mappers;
using InfigoHR.Authentication.Models;
using InfigoHR.Authentication.Options;
using InfigoHR.Domain.Entities;
using InfigoHR.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace InfigoHR.Authentication.Services;

public class TwoFactorService : ITwoFactorService
{
    private readonly ITwoFactorRepository _repository;
    private readonly ITwoFactorTokenProvider _tokenProvider;
    private readonly IPasswordHasher<AuthEntity> _passwordHasher;
    private readonly IJwtProvider _jwtProvider;
    private readonly TwoFactorOptions _options;
    private readonly IValidator<TwoFactorSetupRequest> _setupRequestValidator;
    private readonly IValidator<TwoFactorEnableRequest> _enableRequestValidator;
    private readonly IValidator<TwoFactorChallengeRequest> _challengeRequestValidator;
    private readonly IValidator<TwoFactorManagementRequest> _managementRequestValidator;

    public TwoFactorService(
        ITwoFactorRepository repository,
        ITwoFactorTokenProvider tokenProvider,
        IPasswordHasher<AuthEntity> passwordHasher,
        IJwtProvider jwtProvider,
        IOptions<TwoFactorOptions> options,
        IValidator<TwoFactorSetupRequest> setupRequestValidator,
        IValidator<TwoFactorEnableRequest> enableRequestValidator,
        IValidator<TwoFactorChallengeRequest> challengeRequestValidator,
        IValidator<TwoFactorManagementRequest> managementRequestValidator)
    {
        _repository = repository;
        _tokenProvider = tokenProvider;
        _passwordHasher = passwordHasher;
        _jwtProvider = jwtProvider;
        _options = options.Value;
        _setupRequestValidator = setupRequestValidator;
        _enableRequestValidator = enableRequestValidator;
        _challengeRequestValidator = challengeRequestValidator;
        _managementRequestValidator = managementRequestValidator;
    }

    public async Task<TwoFactorStatusResponse> GetStatusAsync(
        int employeeId,
        Guid tokenVersion,
        CancellationToken cancellationToken = default)
    {
        var employee = await GetEmployeeAsync(
            employeeId,
            tokenVersion,
            cancellationToken);

        return TwoFactorMapper.Status(employee.TwoFactorAuth);
    }

    public async Task<TwoFactorSetupResponse> SetupAsync(
        int employeeId,
        Guid tokenVersion,
        TwoFactorSetupRequest request,
        CancellationToken cancellationToken = default)
    {
        await _setupRequestValidator.ValidateAndThrowAsync(
            request,
            cancellationToken);

        var employee = await GetEmployeeAsync(
            employeeId,
            tokenVersion,
            cancellationToken);

        if (employee.TwoFactorAuth?.IsEnabled == true)
        {
            throw TwoFactorException.AlreadyEnabled();
        }

        employee.TwoFactorAuth ??= new TwoFactorAuthEntity { EmployeeId = employeeId };
        await ReserveAttemptAsync(employee, null, cancellationToken);
        CheckPassword(employee.Auth!, request.Password);

        var state = employee.TwoFactorAuth;
        var secret = _tokenProvider.GenerateSecret();
        state.ProtectedSecret = _tokenProvider.ProtectSecret(
            employee.Id,
            secret);
        state.SetupId = Guid.NewGuid();
        state.SetupTokenVersion = employee.Auth!.TokenVersion;
        state.SetupExpiresAtUtc = DateTime.UtcNow.AddMinutes(
            _options.SetupExpirationMinutes);
        state.LastAcceptedTimeStep = null;
        state.RecoveryCodeHashes = [];
        state.ClearChallenge();

        await SaveAsync(
            employee,
            false,
            state.SetupExpiresAtUtc,
            cancellationToken);

        var authenticatorUri = _tokenProvider.GetAuthenticatorUri(
            employee.Email,
            secret);

        return TwoFactorMapper.Setup(state, secret, authenticatorUri);
    }

    public async Task<TwoFactorRecoveryResponse> EnableAsync(
        int employeeId,
        Guid tokenVersion,
        TwoFactorEnableRequest request,
        CancellationToken cancellationToken = default)
    {
        await _enableRequestValidator.ValidateAndThrowAsync(
            request,
            cancellationToken);

        var employee = await GetEmployeeAsync(
            employeeId,
            tokenVersion,
            cancellationToken);

        var state = employee.TwoFactorAuth;
        var validSetup = state is not null && !state.IsEnabled &&
            state.SetupId == request.SetupId &&
            state.SetupTokenVersion == employee.Auth!.TokenVersion &&
            state.SetupExpiresAtUtc > DateTime.UtcNow;

        if (!validSetup)
        {
            throw TwoFactorException.Invalid();
        }

        var expiresAt = state!.SetupExpiresAtUtc;
        await ReserveAttemptAsync(
            employee,
            expiresAt,
            cancellationToken);

        if (!AcceptProof(employee, request.Code, null))
        {
            throw TwoFactorException.Invalid();
        }

        var codes = ReplaceRecoveryCodes(employee);
        state.IsEnabled = true;
        state.SetupId = Guid.Empty;
        state.SetupTokenVersion = Guid.Empty;
        state.SetupExpiresAtUtc = null;
        state.ClearFailures();
        state.ClearChallenge();
        await SaveAsync(employee, true, expiresAt, cancellationToken);

        var accessToken = GenerateAccessToken(employee);
        return TwoFactorMapper.Recovery(accessToken, codes);
    }

    public async Task<AuthResponse> VerifyAsync(
        TwoFactorChallengeRequest request,
        CancellationToken cancellationToken = default)
    {
        await _challengeRequestValidator.ValidateAndThrowAsync(
            request,
            cancellationToken);

        var hash = _tokenProvider.HashChallenge(request.ChallengeToken);
        var employee = await _repository.GetByChallengeHashAsync(
            hash,
            cancellationToken);

        var state = employee?.TwoFactorAuth;
        var validChallenge = employee?.Auth is not null
                             && employee.Status == EmployeeStatus.Active && state?.IsEnabled == true
                             && state.ChallengeTokenVersion == employee.Auth.TokenVersion
                             && state.ChallengeExpiresAtUtc > DateTime.UtcNow;

        if (!validChallenge)
        {
            throw TwoFactorException.Invalid();
        }

        var expiresAt = state!.ChallengeExpiresAtUtc;
        await ReserveAttemptAsync(
            employee!,
            expiresAt,
            cancellationToken);

        if (!AcceptProof(employee!, request.Code, request.RecoveryCode))
        {
            throw TwoFactorException.Invalid();
        }

        state.ClearFailures();
        state.ClearChallenge();
        await SaveAsync(employee!, false, expiresAt, cancellationToken);

        var accessToken = GenerateAccessToken(employee!);
        return TwoFactorMapper.Authenticated(accessToken);
    }

    public async Task<TwoFactorRecoveryResponse> RegenerateAsync(
        int employeeId,
        Guid tokenVersion,
        TwoFactorManagementRequest request,
        CancellationToken cancellationToken = default)
    {
        var employee = await VerifyManagementAsync(
            employeeId,
            tokenVersion,
            request,
            cancellationToken);

        var codes = ReplaceRecoveryCodes(employee);
        employee.TwoFactorAuth!.ClearFailures();
        employee.TwoFactorAuth.ClearChallenge();
        await SaveAsync(employee, true, null, cancellationToken);

        var accessToken = GenerateAccessToken(employee);
        return TwoFactorMapper.Recovery(accessToken, codes);
    }

    public async Task<AuthResponse> DisableAsync(
        int employeeId,
        Guid tokenVersion,
        TwoFactorManagementRequest request,
        CancellationToken cancellationToken = default)
    {
        var employee = await VerifyManagementAsync(
            employeeId,
            tokenVersion,
            request,
            cancellationToken);

        employee.TwoFactorAuth!.Disable();
        await SaveAsync(employee, true, null, cancellationToken);

        var accessToken = GenerateAccessToken(employee);
        return TwoFactorMapper.Authenticated(accessToken);
    }

    private async Task<EmployeeEntity> VerifyManagementAsync(
        int employeeId,
        Guid tokenVersion,
        TwoFactorManagementRequest request,
        CancellationToken cancellationToken)
    {
        await _managementRequestValidator.ValidateAndThrowAsync(
            request,
            cancellationToken);

        var employee = await GetEmployeeAsync(
            employeeId,
            tokenVersion,
            cancellationToken);

        if (employee.TwoFactorAuth?.IsEnabled != true)
        {
            throw TwoFactorException.Invalid();
        }

        await ReserveAttemptAsync(employee, null, cancellationToken);
        CheckPassword(employee.Auth!, request.Password);

        if (!AcceptProof(employee, request.Code, request.RecoveryCode))
        {
            throw TwoFactorException.Invalid();
        }

        return employee;
    }

    private async Task<EmployeeEntity> GetEmployeeAsync(
        int employeeId,
        Guid tokenVersion,
        CancellationToken cancellationToken)
    {
        var employee = await _repository.GetByIdAsync(
            employeeId,
            cancellationToken);

        var valid = employee?.Auth is not null &&
            employee.Status == EmployeeStatus.Active &&
            employee.Auth.TokenVersion == tokenVersion;
        if (!valid)
        {
            throw TwoFactorException.Invalid();
        }

        return employee!;
    }

    private void CheckPassword(AuthEntity auth, string password)
    {
        var result = _passwordHasher.VerifyHashedPassword(
            auth,
            auth.PasswordHash,
            password);

        if (result == PasswordVerificationResult.Failed)
        {
            throw TwoFactorException.Invalid();
        }
    }

    private bool AcceptProof(
        EmployeeEntity employee,
        string? code,
        string? recoveryCode)
    {
        var state = employee.TwoFactorAuth!;
        if (recoveryCode is not null)
        {
            return state.TryConsumeRecoveryCode(_tokenProvider.HashRecoveryCode(employee.Id, recoveryCode));
        }

        var valid = _tokenProvider.TryVerifyCode(
            employee.Id,
            state.ProtectedSecret,
            code!,
            DateTime.UtcNow,
            out var step);

        return valid && state.TryAcceptTimeStep(step);
    }

    private string[] ReplaceRecoveryCodes(EmployeeEntity employee)
    {
        var codes = _tokenProvider.GenerateRecoveryCodes();

        employee.TwoFactorAuth!.RecoveryCodeHashes = codes
            .Select(code => _tokenProvider.HashRecoveryCode(employee.Id, code))
            .ToArray();

        return codes;
    }

    private async Task ReserveAttemptAsync(
        EmployeeEntity employee,
        DateTime? validUntil,
        CancellationToken cancellationToken)
    {
        var state = employee.TwoFactorAuth!;
        var now = DateTime.UtcNow;

        if (state.IsLocked(now))
        {
            throw TwoFactorException.Invalid();
        }

        state.RegisterFailure(
            now,
            _options.MaximumAttempts,
            _options.LockoutMinutes);

        await SaveAsync(employee, false, validUntil, cancellationToken);
    }

    private async Task SaveAsync(
        EmployeeEntity employee,
        bool rotateTokenVersion,
        DateTime? validUntil,
        CancellationToken cancellationToken)
    {
        var auth = employee.Auth!;
        var nextTokenVersion = rotateTokenVersion ? Guid.NewGuid() : auth.TokenVersion;
        var saved = await _repository.TrySaveAsync(
            employee,
            auth.ConcurrencyVersion,
            nextTokenVersion,
            validUntil,
            cancellationToken);

        if (!saved)
        {
            throw TwoFactorException.Conflict();
        }
    }

    private string GenerateAccessToken(EmployeeEntity employee)
    {
        var permissions = employee.Roles
            .SelectMany(role => role.Permissions)
            .Select(permission => permission.Name);

        return _jwtProvider.Generate(
            employee.Id,
            employee.Email,
            employee.Auth!.TokenVersion,
            permissions,
            employee.Roles.Select(role => role.Name));
    }
}
