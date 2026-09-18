using InfigoHR.Authentication.Jwt;
using InfigoHR.Authentication.Models;
using InfigoHR.Domain.Entities;
using InfigoHR.Domain.Enums;
using InfigoHR.Persistence.Repositories;
using Microsoft.AspNetCore.Identity;

namespace InfigoHR.Authentication.Services;

public sealed class AuthService
{
    private readonly AuthRepository _repository;
    private readonly IPasswordHasher<AuthEntity> _passwordHasher;
    private readonly JwtProvider _jwtProvider;

    public AuthService(
        AuthRepository repository,
        IPasswordHasher<AuthEntity> passwordHasher,
        JwtProvider jwtProvider)
    {
        _repository = repository;
        _passwordHasher = passwordHasher;
        _jwtProvider = jwtProvider;
    }

    public async Task<AuthResponse?> SignInAsync(
        SignInRequest request,
        CancellationToken cancellationToken = default)
    {
        var employee = await _repository.GetByEmailAsync(
            request.Email.Trim(),
            cancellationToken);

        if (employee?.Auth is null ||
            employee.Status != EmployeeStatus.Active)
        {
            return null;
        }

        var result = _passwordHasher.VerifyHashedPassword(
            employee.Auth,
            employee.Auth.PasswordHash,
            request.Password);

        if (result == PasswordVerificationResult.Failed)
        {
            return null;
        }

        return new AuthResponse(_jwtProvider.Generate(employee));
    }

    public async Task<AuthResponse?> SignUpAsync(
        SignUpRequest request,
        CancellationToken cancellationToken = default)
    {
        var email = request.Email.Trim();

        if (await _repository.EmailExistsAsync(
                email,
                cancellationToken))
        {
            return null;
        }

        var employee = new EmployeeEntity
        {
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            Email = email,
            Title = request.Title.Trim(),
            StartedOnUtc = request.StartedOn!.Value.UtcDateTime,
            Status = EmployeeStatus.Active
        };

        var auth = new AuthEntity
        {
            Employee = employee
        };

        auth.PasswordHash = _passwordHasher.HashPassword(
            auth,
            request.Password);

        employee.Auth = auth;

        var created = await _repository.TryAddAsync(
            employee,
            cancellationToken);

        if (!created)
        {
            return null;
        }

        return new AuthResponse(_jwtProvider.Generate(employee));
    }
}