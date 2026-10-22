using InfigoHR.Application.Interfaces.Repositories;
using InfigoHR.Authentication.Exceptions;
using InfigoHR.Authentication.Interfaces;
using InfigoHR.Authentication.Mappers;
using InfigoHR.Authentication.Models;
using InfigoHR.Authentication.Options;
using InfigoHR.Domain.Entities;
using Microsoft.Extensions.Options;

namespace InfigoHR.Authentication.Services;

public class SignInCompletionService : ISignInCompletionService
{
    private readonly ITwoFactorRepository _repository;
    private readonly ITwoFactorTokenProvider _tokenProvider;
    private readonly IJwtProvider _jwtProvider;
    private readonly TwoFactorOptions _options;

    public SignInCompletionService(
        ITwoFactorRepository repository,
        ITwoFactorTokenProvider tokenProvider,
        IJwtProvider jwtProvider,
        IOptions<TwoFactorOptions> options)
    {
        _repository = repository;
        _tokenProvider = tokenProvider;
        _jwtProvider = jwtProvider;
        _options = options.Value;
    }

    public async Task<AuthResponse> CompleteAsync(
        EmployeeEntity employee,
        CancellationToken cancellationToken = default)
    {
        var auth = employee.Auth ?? throw TwoFactorException.Invalid();
        var state = employee.TwoFactorAuth;
        var now = DateTime.UtcNow;
        string? challenge = null;

        if (state?.IsEnabled == true)
        {
            if (state.IsLocked(now))
            {
                throw TwoFactorException.Invalid();
            }

            challenge = _tokenProvider.GenerateChallenge();
            state.ChallengeTokenHash = _tokenProvider.HashChallenge(challenge);
            state.ChallengeTokenVersion = auth.TokenVersion;
            state.ChallengeExpiresAtUtc = now.AddMinutes(
                _options.ChallengeExpirationMinutes);
        }

        var saved = await _repository.TrySaveAsync(
            employee,
            auth.ConcurrencyVersion,
            auth.TokenVersion,
            null,
            cancellationToken);
        if (!saved)
        {
            throw TwoFactorException.Conflict();
        }

        if (challenge is not null)
        {
            return TwoFactorMapper.Challenge(
                challenge,
                state!.ChallengeExpiresAtUtc!.Value);
        }

        var permissions = employee.Roles
            .SelectMany(role => role.Permissions)
            .Select(permission => permission.Name);

        var accessToken = _jwtProvider.Generate(
            employee.Id,
            employee.Email,
            auth.TokenVersion,
            permissions,
            employee.Roles.Select(role => role.Name));

        return TwoFactorMapper.Authenticated(accessToken);
    }
}
