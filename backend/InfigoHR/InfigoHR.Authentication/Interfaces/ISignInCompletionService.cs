using InfigoHR.Authentication.Models;
using InfigoHR.Domain.Entities;

namespace InfigoHR.Authentication.Interfaces;

public interface ISignInCompletionService
{
    Task<AuthResponse> CompleteAsync(
        EmployeeEntity employee,
        CancellationToken cancellationToken = default);
}
