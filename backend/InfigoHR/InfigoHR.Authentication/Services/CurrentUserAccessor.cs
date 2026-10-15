using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using InfigoHR.Application.Exceptions;
using InfigoHR.Application.Interfaces.Services;
using Microsoft.AspNetCore.Http;

namespace InfigoHR.Authentication.Services;

public class CurrentUserAccessor : ICurrentUserAccessor
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserAccessor(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal User
    {
        get
        {
            var user = _httpContextAccessor.HttpContext?.User;

            if (user?.Identity?.IsAuthenticated != true)
            {
                throw new UnauthorizedException("An authenticated user is required.");
            }

            return user;
        }
    }

    public int EmployeeId
    {
        get
        {
            var subject = User.FindFirstValue(JwtRegisteredClaimNames.Sub);

            if (!int.TryParse(subject, out var employeeId) || employeeId <= 0)
            {
                throw new UnauthorizedException("A valid employee ID is required.");
            }

            return employeeId;
        }
    }

    public IReadOnlyCollection<string> Roles
    {
        get
        {
            var roles = User.FindAll("role")
                .Select(claim => claim.Value)
                .ToArray();

            return roles.Length > 0
                ? roles
                : throw new UnauthorizedException("A user role is required.");
        }
    }

    public bool IsInRole(string role) => User.IsInRole(role);
}
