using System.Security.Claims;

namespace GameVault.Server.Services;

public interface ICurrentUserService
{
    string? UserId { get; }
    string? Email { get; }
    string? Role { get; }
    bool IsAuthenticated { get; }
}

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? UserId => _httpContextAccessor.HttpContext?.User
        .FindFirst(ClaimTypes.NameIdentifier)?.Value
        ?? _httpContextAccessor.HttpContext?.User.FindFirst("user_id")?.Value;

    public string? Email => _httpContextAccessor.HttpContext?.User
        .FindFirst(ClaimTypes.Email)?.Value;

    public string? Role => _httpContextAccessor.HttpContext?.User
        .FindFirst(ClaimTypes.Role)?.Value;

    public bool IsAuthenticated => 
        _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
}