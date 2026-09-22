using FluentValidation;
using InfigoHR.Application.Common;
using InfigoHR.Application.Exceptions;
using InfigoHR.Application.Interfaces.Repositories;
using InfigoHR.Application.Interfaces.Services;
using InfigoHR.Application.Mappers;
using InfigoHR.Application.Models;
using InfigoHR.Application.QueryParameters.Employees;

namespace InfigoHR.Application.Services;

using EmailService;
using Microsoft.Extensions.Configuration;

public class EmployeeService(
    IEmployeeRepository repository,
    IValidator<CreateEmployeeModel> createValidator,
    IValidator<UpdateEmployeeModel> updateValidator,
    InvitationLink invitationLink,
    IConfiguration configuration) : IEmployeeService
{
    private readonly InvitationLink _invitationLink = invitationLink;
    private readonly string _frontendBaseUrl = configuration["FrontendBaseUrl"] ?? "http://localhost:3000/register";

    public async Task<PagedResult<EmployeeModel>> GetAllAsync(EmployeeParameters query)
    {
        var pagedList = await repository.SearchAsync(query);
        return pagedList.ToPagedResult(EmployeeMapper.ToModel);
    }

    public async Task<EmployeeModel> GetByIdAsync(int id)
    {
        var entity = await repository.GetByIdAsync(id)
                     ?? throw new NotFoundException($"Employee with id {id} was not found.");
        return EmployeeMapper.ToModel(entity);
    }

    public async Task<int> CreateAsync(CreateEmployeeModel model)
    {
        await createValidator.ValidateAndThrowAsync(model);

        var entity = EmployeeMapper.ToEntity(model);
        await repository.AddAsync(entity);

        // If employee is inactive, send invitation link automatically
        if (model.Status == Domain.Enums.EmployeeStatus.Inactive)
        {
            try
            {
                var created = await _invitationLink.SendAsync(entity.Email, _frontendBaseUrl);

                // Extract token query parameter from the returned URL
                var token = ExtractTokenFromUrl(created.Url);

                entity.InviteToken = token;
                entity.InviteTokenExpiresAtUtc = created.ExpiresAtUtc.UtcDateTime;

                await repository.UpdateAsync(entity);
            }
            catch
            {
                // Swallowing exception intentionally: invitation failure shouldn't prevent creation
                // Consider logging the error in Presentation layer
            }
        }

        return entity.Id;
    }

    public async Task UpdateAsync(UpdateEmployeeModel model)
    {
        await updateValidator.ValidateAndThrowAsync(model);

        var entity = await repository.GetByIdAsync(model.Id)
                     ?? throw new NotFoundException($"Employee with id {model.Id} was not found.");

        EmployeeMapper.ToEntity(model, entity);
        await repository.UpdateAsync(entity);
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await repository.GetByIdAsync(id)
                     ?? throw new NotFoundException($"Employee with id {id} was not found.");
        await repository.DeleteAsync(entity);
    }

    private static string? ExtractTokenFromUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(url)) return null;
        var idx = url.IndexOf("token=", StringComparison.Ordinal);
        if (idx < 0) return null;
        var tokenPart = url[(idx + "token=".Length) ..];
        // token may be followed by & or fragment
        var amp = tokenPart.IndexOf('&');
        if (amp >= 0) tokenPart = tokenPart[..amp];
        var hashIdx = tokenPart.IndexOf('#');
        if (hashIdx >= 0) tokenPart = tokenPart[..hashIdx];
        return Uri.UnescapeDataString(tokenPart);
    }
}