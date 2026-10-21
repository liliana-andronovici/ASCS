using FluentValidation;
using InfigoHR.Application.Interfaces.Repositories;
using InfigoHR.Authentication.Exceptions;
using InfigoHR.Authentication.Interfaces;
using InfigoHR.Authentication.Models;
using InfigoHR.Authentication.Options;
using InfigoHR.Domain.Entities;
using InfigoHR.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace InfigoHR.Authentication.Services;

public class PasswordResetService : IPasswordResetService
{
    private readonly IAuthRepository _authRepository;
    private readonly IEmployeeRepository _repository;
    private readonly IPasswordResetTokenProvider _tokenProvider;
    private readonly IPasswordHasher<AuthEntity> _passwordHasher;
    private readonly ISignInCompletionService _signInCompletion;
    private readonly IPasswordResetEmailService _emailService;
    private readonly PasswordResetOptions _options;
    private readonly IValidator<RequestPasswordResetRequest> _requestValidator;
    private readonly IValidator<VerifyPasswordResetRequest> _verifyValidator;
    private readonly IValidator<CompletePasswordResetRequest> _completeValidator;

    public PasswordResetService(
        IAuthRepository authRepository,
        IEmployeeRepository repository,
        IPasswordResetTokenProvider tokenProvider,
        IPasswordHasher<AuthEntity> passwordHasher,
        ISignInCompletionService signInCompletion,
        IPasswordResetEmailService emailService,
        IOptions<PasswordResetOptions> options,
        IValidator<RequestPasswordResetRequest> requestValidator,
        IValidator<VerifyPasswordResetRequest> verifyValidator,
        IValidator<CompletePasswordResetRequest> completeValidator)
    {
        _authRepository = authRepository;
        _repository = repository;
        _tokenProvider = tokenProvider;
        _passwordHasher = passwordHasher;
        _signInCompletion = signInCompletion;
        _emailService = emailService;
        _options = options.Value;
        _requestValidator = requestValidator;
        _verifyValidator = verifyValidator;
        _completeValidator = completeValidator;
    }

    public async Task RequestAsync(
        RequestPasswordResetRequest request,
        CancellationToken cancellationToken = default)
    {
        await _requestValidator.ValidateAndThrowAsync(
            request,
            cancellationToken);

        var employee = await GetActiveEmployeeAsync(
            request.Email,
            cancellationToken);

        if (employee is null)
        {
            return;
        }

        var auth = employee.Auth!;
        var now = DateTime.UtcNow;

        var resendAllowedAt = auth.ResetRequestedAtUtc?
            .AddSeconds(_options.ResendCooldownSeconds);

        var isResendTooSoon =
            resendAllowedAt.HasValue && now < resendAllowedAt.Value;

        if (isResendTooSoon)
        {
            return;
        }

        var hasActiveCodeWindow =
            auth.ResetCodeExpiresAtUtc.HasValue &&
            auth.ResetCodeExpiresAtUtc.Value > now;

        var attemptsExhausted =
            auth.ResetAttempts >= _options.MaxVerificationAttempts;

        if (hasActiveCodeWindow && attemptsExhausted)
        {
            return;
        }

        var code = _tokenProvider.GenerateCode();

        if (!hasActiveCodeWindow)
        {
            auth.ResetAttempts = 0;
            auth.ResetCodeExpiresAtUtc = now.AddMinutes(
                _options.CodeExpirationMinutes);
        }

        auth.ResetCodeHash = _tokenProvider.HashCode(employee.Id, code);
        auth.ResetRequestedAtUtc = now;

        auth.ResetToken = null;
        auth.ResetTokenExpiresAtUtc = null;

        var saved = await TrySaveAuthenticationAsync(auth, cancellationToken);

        if (!saved)
        {
            return;
        }

        var expiresAt = auth.ResetCodeExpiresAtUtc!.Value;

        await _emailService.SendCodeAsync(
            employee.Email,
            code,
            expiresAt,
            cancellationToken);
    }

    public async Task<VerifyPasswordResetResponse> VerifyAsync(
        VerifyPasswordResetRequest request,
        CancellationToken cancellationToken = default)
    {
        await _verifyValidator.ValidateAndThrowAsync(
            request,
            cancellationToken);

        var employee = await GetActiveEmployeeAsync(
            request.Email,
            cancellationToken);

        if (employee is null)
        {
            throw PasswordResetException.InvalidCode();
        }

        var auth = employee.Auth!;

        EnsureCodeCanBeVerified(auth);

        await ReserveAttemptAndVerifyCodeAsync(
            auth,
            employee.Id,
            request.Code,
            cancellationToken);

        var resetToken = await CreateResetTokenAsync(
            auth,
            cancellationToken);

        return new VerifyPasswordResetResponse
        {
            ResetToken = resetToken
        };
    }

    public async Task<AuthResponse> CompleteAsync(
        CompletePasswordResetRequest request,
        CancellationToken cancellationToken = default)
    {
        await _completeValidator.ValidateAndThrowAsync(
            request,
            cancellationToken);

        var employee = await GetActiveEmployeeAsync(
            request.Email,
            cancellationToken);

        if (employee is null)
        {
            throw PasswordResetException.InvalidToken();
        }

        var auth = employee.Auth!;
        var now = DateTime.UtcNow;

        var hasResetToken = auth.ResetToken is not null;
        var isExpired =
            !auth.ResetTokenExpiresAtUtc.HasValue ||
            auth.ResetTokenExpiresAtUtc.Value <= now;

        if (!hasResetToken || isExpired)
        {
            throw PasswordResetException.InvalidToken();
        }

        var submittedHash = _tokenProvider.HashToken(request.ResetToken);

        var matches = _tokenProvider.Matches(
            auth.ResetToken!,
            submittedHash);

        if (!matches)
        {
            throw PasswordResetException.InvalidToken();
        }

        ApplyPasswordChange(auth, request.NewPassword);

        var saved = await TrySaveAuthenticationAsync(auth, cancellationToken);

        if (!saved)
        {
            throw PasswordResetException.InvalidToken();
        }

        await _emailService.SendPasswordChangedAsync(
            employee.Email,
            cancellationToken);

        return await _signInCompletion.CompleteAsync(
            employee,
            cancellationToken);
    }

    private void ApplyPasswordChange(AuthEntity auth, string newPassword)
    {
        auth.PasswordHash = _passwordHasher.HashPassword(auth, newPassword);
        auth.TokenVersion = Guid.NewGuid();

        auth.ResetToken = null;
        auth.ResetTokenExpiresAtUtc = null;
        auth.ResetCodeHash = null;
        auth.ResetCodeExpiresAtUtc = null;
        auth.ResetAttempts = 0;
    }

    private async Task<EmployeeEntity?> GetActiveEmployeeAsync(
        string email,
        CancellationToken cancellationToken)
    {
        var employee = await _repository.GetByEmailAsync(
            email,
            cancellationToken);

        if (employee?.Auth is null)
        {
            return null;
        }

        var isActive = employee.Status == EmployeeStatus.Active;

        return isActive ? employee : null;
    }

    private Task<bool> TrySaveAuthenticationAsync(
        AuthEntity auth,
        CancellationToken cancellationToken)
    {
        var expectedConcurrencyVersion = auth.ConcurrencyVersion;

        auth.ConcurrencyVersion = Guid.NewGuid();
        auth.UpdatedOnUtc = DateTime.UtcNow;

        return _authRepository.TryUpdateAuthenticationAsync(
            auth,
            expectedConcurrencyVersion,
            cancellationToken);
    }

    private void EnsureCodeCanBeVerified(AuthEntity auth)
    {
        var hasCode = auth.ResetCodeHash is not null;

        var isExpired =
            !auth.ResetCodeExpiresAtUtc.HasValue ||
            auth.ResetCodeExpiresAtUtc.Value <= DateTime.UtcNow;

        var attemptsExhausted =
            auth.ResetAttempts >= _options.MaxVerificationAttempts;

        if (!hasCode || isExpired || attemptsExhausted)
        {
            throw PasswordResetException.InvalidCode();
        }
    }

    private async Task ReserveAttemptAndVerifyCodeAsync(
        AuthEntity auth,
        int employeeId,
        string code,
        CancellationToken cancellationToken)
    {
        auth.ResetAttempts++;

        var attemptReserved = await TrySaveAuthenticationAsync(
            auth,
            cancellationToken);

        if (!attemptReserved)
        {
            throw PasswordResetException.InvalidCode();
        }

        var submittedHash = _tokenProvider.HashCode(employeeId, code);

        var matches = _tokenProvider.Matches(
            auth.ResetCodeHash!,
            submittedHash);

        if (!matches)
        {
            throw PasswordResetException.InvalidCode();
        }
    }

    private async Task<string> CreateResetTokenAsync(
        AuthEntity auth,
        CancellationToken cancellationToken)
    {
        var resetToken = _tokenProvider.GenerateToken();

        auth.ResetCodeHash = null;
        auth.ResetToken = _tokenProvider.HashToken(resetToken);
        auth.ResetTokenExpiresAtUtc = DateTime.UtcNow.AddMinutes(
            _options.ResetTokenExpirationMinutes);

        var saved = await TrySaveAuthenticationAsync(
            auth,
            cancellationToken);

        if (!saved)
        {
            throw PasswordResetException.InvalidCode();
        }

        return resetToken;
    }
}