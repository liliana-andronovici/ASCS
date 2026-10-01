using InfigoHR.Authentication.Models;

namespace InfigoHR.Authentication.Interfaces;

public interface IAuthService
{
    Task<AuthResponse?> SignInAsync(
        SignInRequest request,
        CancellationToken cancellationToken = default);

    Task SignUpAsync(
        SignUpRequest request,
        CancellationToken cancellationToken = default);
}