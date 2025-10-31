using System.Net.Http.Json;
using System.Security.Principal;
using System.Text.Json;
using GameVault.Shared.DTOs;
using GameVault.Shared.Models;

namespace GameVault.Client.Services;

public class AuthService
{
    private readonly HttpClient _httpClient;
    private string? _currentUserId;
    private string? _currentToken;
    private UserDTO? _currentUser;

    public AuthService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public bool IsAuthenticated => !string.IsNullOrEmpty(_currentToken);
    public string? CurrentUserId => _currentUserId;
    public UserDTO? CurrentUser => _currentUser;
    public string? Token => _currentToken;

    public async Task<AuthResponse> LoginAsync(string email, string password)
    {
        var request = new
        {
            Email = email,
            Password = password
        };

        var response = await _httpClient.PostAsJsonAsync("api/auth/login", request);
        var result = await response.Content.ReadFromJsonAsync<AuthResponse>();

        if (result?.Success == true && result.IdToken != null)
        {
            _currentToken = result.IdToken;
            _currentUserId = result.Data?.Id;
            _currentUser = result.Data;

            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _currentToken);
        }

        return result ?? new AuthResponse { Success = false, Message = "Unknown error" };
    }

    public async Task<AuthResponse> RegisterCustomerAsync(string email, string password) //, string? displayName)
    {
        var request = new RegisterCustomerRequest
        {
            Email = email,
            Password = password,
            // DisplayName = displayName
        };

        var response = await _httpClient.PostAsJsonAsync("api/auth/register/customer", request);
        var result = await response.Content.ReadFromJsonAsync<AuthResponse>();

        if (result?.Success == true && result.IdToken != null)
        {
            // Don't set token because not actually logged in on the server side
            // _currentToken = result.IdToken;
            _currentUserId = result.Data?.Id ?? null;
            _currentUser = result.Data;

            // Not actually logged in on the server side
            // _httpClient.DefaultRequestHeaders.Authorization =
            //     new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _currentToken);
        }

        return result ?? new AuthResponse { Success = false, Message = "Unknown error" };
    }

    public async Task<DataResponse<string>> RegisterVendorAsync(
        string email,
        string password,
        string displayName,
        string reason)
    {
        var request = new RegisterVendorRequest
        {
            Email = email,
            Password = password,
            DisplayName = displayName,
            Reason = reason
        };

        var response = await _httpClient.PostAsJsonAsync("api/auth/register/vendor", request);
        var result = await response.Content.ReadFromJsonAsync<DataResponse<string>>();

        // Don't do any of this because account not really created
        //      && result.IdToken != null)
        // {
        //     _currentToken = result.IdToken;
        //     _currentUserId = result.UserId;
        //     _currentUser = result.User;

        //     _httpClient.DefaultRequestHeaders.Authorization =
        //         new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _currentToken);
        // }

        return result ?? new DataResponse<string> { Success = false, Message = "Unknown error" };
    }

    public void Logout()
    {
        _currentToken = null;
        _currentUserId = null;
        _currentUser = null;
        _httpClient.DefaultRequestHeaders.Authorization = null;
    }
}