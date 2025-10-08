using System.Net.Http.Json;
using System.Text.Json;

namespace GameVault.Client.Services;

public class AuthService
{
    private readonly HttpClient _httpClient;
    private string? _currentUserId;
    private string? _currentToken;

    public AuthService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public bool IsAuthenticated => !string.IsNullOrEmpty(_currentToken);
    public string? CurrentUserId => _currentUserId;

    public async Task<AuthResponse> RegisterCustomerAsync(string email, string password, string? displayName)
    {
        var request = new
        {
            Email = email,
            Password = password,
            DisplayName = displayName
        };

        var response = await _httpClient.PostAsJsonAsync("api/auth/register/customer", request);
        var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
        
        if (result?.Success == true && result.IdToken != null)
        {
            _currentToken = result.IdToken;
            _currentUserId = result.UserId;
        }

        return result ?? new AuthResponse { Success = false, Message = "Unknown error" };
    }

    public async Task<AuthResponse> RegisterVendorAsync(
        string email, 
        string password, 
        string displayName,
        string businessName,
        string businessDescription)
    {
        var request = new
        {
            Email = email,
            Password = password,
            DisplayName = displayName,
            BusinessName = businessName,
            BusinessDescription = businessDescription
        };

        var response = await _httpClient.PostAsJsonAsync("api/auth/register/vendor", request);
        var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
        
        if (result?.Success == true && result.IdToken != null)
        {
            _currentToken = result.IdToken;
            _currentUserId = result.UserId;
        }

        return result ?? new AuthResponse { Success = false, Message = "Unknown error" };
    }

    public void Logout()
    {
        _currentToken = null;
        _currentUserId = null;
    }
}


public class AuthResponse
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public string? UserId { get; set; }
    public string? IdToken { get; set; }
    public UserProfile? User { get; set; }
}

public class UserProfile
{
    public string UserId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? DisplayName { get; set; }
    public string Role { get; set; } = string.Empty;
    public string? ApprovalStatus { get; set; }
    public string? BusinessName { get; set; }
}