using Transport.Application.DTOs.Auth;

namespace Transport.Application.Services;

public interface IAuthService
{
    Task<LoginResponse?> LoginAsync(LoginRequest request);
    Task<LoginResponse?> RefreshTokenAsync(string token, string refreshToken);
}

