using System.Net.Http.Json;
using GameVault.Shared.DTOs;
using GameVault.Shared.Models;
using Microsoft.JSInterop;

namespace GameVault.Client.Services;

public class AuthService
{
    private readonly HttpClient _httpClient;
    private readonly IJSRuntime _js;
    private string? _currentUserId;
    private string? _currentToken;
    private UserDTO? _currentUser;

    public event Action? OnAuthStateChanged;

    public AuthService(HttpClient httpClient, IJSRuntime js)
    {
        _httpClient = httpClient;
        _js = js;
    }

    public bool IsAuthenticated => !string.IsNullOrEmpty(_currentToken);
    public string? CurrentUserId => _currentUserId;
    public UserDTO? CurrentUser => _currentUser;
    public string? Token => _currentToken;

    public async Task InitializeAsync()
    {
        try
        {
            var token = await _js.InvokeAsync<string?>("localStorage.getItem", "authToken");
            var userId = await _js.InvokeAsync<string?>("localStorage.getItem", "userId");
            var userJson = await _js.InvokeAsync<string?>("localStorage.getItem", "currentUser");

            if (!string.IsNullOrEmpty(token) && !string.IsNullOrEmpty(userId))
            {
                _currentToken = token;
                _currentUserId = userId;
                
                if (!string.IsNullOrEmpty(userJson))
                {
                    _currentUser = System.Text.Json.JsonSerializer.Deserialize<UserDTO>(userJson);
                }

                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _currentToken);

                OnAuthStateChanged?.Invoke();
            }
        }
        catch
        {
            // Silent fail - user just won't be logged in
        }
    }

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

            await _js.InvokeVoidAsync("localStorage.setItem", "authToken", _currentToken);
            await _js.InvokeVoidAsync("localStorage.setItem", "userId", _currentUserId);
            
            if (_currentUser != null)
            {
                var userJson = System.Text.Json.JsonSerializer.Serialize(_currentUser);
                await _js.InvokeVoidAsync("localStorage.setItem", "currentUser", userJson);
            }
            
            OnAuthStateChanged?.Invoke();
        }

        return result ?? new AuthResponse { Success = false, Message = "Unknown error" };
    }

    public async Task<AuthResponse> RegisterCustomerAsync(string email, string password)
    {
        var request = new RegisterCustomerRequest
        {
            Email = email,
            Password = password,
        };

        var response = await _httpClient.PostAsJsonAsync("api/auth/register/customer", request);
        var result = await response.Content.ReadFromJsonAsync<AuthResponse>();

        if (result?.Success == true && result.IdToken != null)
        {
            _currentUserId = result.Data?.Id ?? null;
            _currentUser = result.Data;
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

        return result ?? new DataResponse<string> { Success = false, Message = "Unknown error" };
    }

    public async Task Logout()
    {
        _currentToken = null;
        _currentUserId = null;
        _currentUser = null;
        _httpClient.DefaultRequestHeaders.Authorization = null;

        await _js.InvokeVoidAsync("localStorage.removeItem", "authToken");
        await _js.InvokeVoidAsync("localStorage.removeItem", "userId");
        await _js.InvokeVoidAsync("localStorage.removeItem", "currentUser");
        
        OnAuthStateChanged?.Invoke();
    }
}