using System.Security.Claims;

namespace Transport.Application.Interfaces;

public interface IJwtService
{
    string GenerateToken(string email, string role, Guid userId);
    string GenerateRefreshToken();
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
}

