using FluentValidation;
using InfigoHR.Application.Interfaces.Repositories;
using InfigoHR.Authentication.Interfaces;
using InfigoHR.Authentication.Mappers;
using InfigoHR.Authentication.Models;
using InfigoHR.Domain.Entities;
using InfigoHR.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace InfigoHR.Authentication.Services;

public class AuthService : IAuthService
{
    private readonly IEmployeeRepository _repository;
    private readonly IPasswordHasher<AuthEntity> _passwordHasher;
    private readonly IJwtProvider _jwtProvider;
    private readonly IValidator<SignInRequest> _signInValidator;
    private readonly IValidator<SignUpRequest> _signUpValidator;

    public AuthService(
        IEmployeeRepository repository,
        IPasswordHasher<AuthEntity> passwordHasher,
        IJwtProvider jwtProvider,
        IValidator<SignInRequest> signInValidator,
        IValidator<SignUpRequest> signUpValidator)
    {
        _repository = repository;
        _passwordHasher = passwordHasher;
        _jwtProvider = jwtProvider;
        _signInValidator = signInValidator;
        _signUpValidator = signUpValidator;
    }

    public async Task<AuthResponse?> SignInAsync(
        SignInRequest request,
        CancellationToken cancellationToken = default)
    {
        await _signInValidator.ValidateAndThrowAsync(
            request,
            cancellationToken);

        var employee = await _repository.GetByEmailAsync(
            request.Email,
            cancellationToken);

        var auth = employee?.Auth;

        if (employee is null || auth is null)
        {
            return null;
        }

        var isActive = employee.Status == EmployeeStatus.Active;

        if (!isActive)
        {
            return null;
        }

        var verificationResult = _passwordHasher.VerifyHashedPassword(
            auth,
            auth.PasswordHash,
            request.Password);

        var isPasswordInvalid =
            verificationResult == PasswordVerificationResult.Failed;

        if (isPasswordInvalid)
        {
            return null;
        }

        var roles = employee.Roles;

        var permissions = roles
            .SelectMany(role => role.Permissions)
            .Select(permission => permission.Name);

        var accessToken = _jwtProvider.Generate(
            employee.Id,
            employee.Email,
            auth.TokenVersion,
            permissions,
            roles.Select(role => role.Name));

        return new AuthResponse
        {
            AccessToken = accessToken
        };
    }

    public async Task SignUpAsync(
        SignUpRequest request,
        CancellationToken cancellationToken = default)
    {
        await _signUpValidator.ValidateAndThrowAsync(
            request,
            cancellationToken);

        var employee = AuthMapper.ToEntity(request);

        var auth = new AuthEntity
        {
            Employee = employee
        };

        auth.PasswordHash = _passwordHasher.HashPassword(
            auth,
            request.Password);

        employee.Auth = auth;

        await _repository.AddAsync(employee);
    }
}