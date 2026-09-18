using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using InfigoHR.Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace InfigoHR.Authentication.Jwt;

public sealed class JwtProvider
{
    private readonly JwtOptions _options;

    public JwtProvider(IOptions<JwtOptions> options)
    {
        _options = options.Value;
    }

    public string Generate(EmployeeEntity employee)
    {
        var claims = new List<Claim>
        {
            new(
                JwtRegisteredClaimNames.Sub,
                employee.Id.ToString()),
            new(
                JwtRegisteredClaimNames.Email,
                employee.Email)
        };

        var permissions = employee.Roles
            .SelectMany(role => role.Permissions)
            .Select(permission => permission.Name)
            .Distinct();

        foreach (var permission in permissions)
        {
            claims.Add(new Claim("permissions", permission));
        }

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_options.SecretKey));

        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256);

        var now = DateTime.UtcNow;

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            notBefore: now,
            expires: now.AddMinutes(_options.ExpirationMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}