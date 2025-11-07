using GameVault.Shared.DTOs;
using GameVault.Shared.Models;
using Google.Apis.Auth.OAuth2;
using System.Net.Http.Json;
using System.Security.Principal;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace GameVault.Client.Services;

public class AuthService
{
    private readonly HttpClient _httpClient;
    private readonly CookieService _cookies;
    private readonly IConfiguration _configuration;
    private string? _apiKey;
    private string? _currentUserId;
    private string? _currentToken;
    private UserDTO? _currentUser;

    public event Action? OnAuthStateChanged;

    public AuthService(HttpClient httpClient, IConfiguration configuration, CookieService cookies)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _apiKey = _configuration["Firebase:ApiKey"];


        _cookies = cookies;
    }

    public bool IsAuthenticated => !string.IsNullOrEmpty(_currentToken);
    public string? CurrentUserId => _currentUserId;
    public UserDTO? CurrentUser => _currentUser;
    public string? Token => _currentToken;

    public async Task InitializeAsync()
    {
        try
        {
            var token = await _cookies.GetCookieAsync("authToken");
            var userId = await _cookies.GetCookieAsync("userId");
            var userJson = await _cookies.GetCookieAsync("currentUser");

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
            // Silent fail
        }
    }

    // Helper classes for Firebase API responses
    private class FirebaseAuthResponse
    {
        public string IdToken { get; set; } = "";
        public string Email { get; set; } = "";
        public string RefreshToken { get; set; } = "";
        public string ExpiresIn { get; set; } = "";
        public string LocalId { get; set; } = "";
    }

    private class FirebaseErrorResponse
    {
        public FirebaseError? Error { get; set; }
    }

    private class FirebaseError
    {
        public int Code { get; set; }
        public string? Message { get; set; }
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

            await _cookies.SetCookieAsync("authToken", _currentToken, 30);
            await _cookies.SetCookieAsync("userId", _currentUserId ?? "", 30);
            
            if (_currentUser != null)
            {
                var userJson = System.Text.Json.JsonSerializer.Serialize(_currentUser);
                await _cookies.SetCookieAsync("currentUser", userJson, 30);
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
        string Id,
        string email,
        string password,
        string displayName,
        string reason)
    {
        var request = new RegisterVendorRequest
        {
            Id = Id,
            Email = email,
            Password = password,
            DisplayName = displayName,
            Reason = reason
        };

        var response = await _httpClient.PostAsJsonAsync("api/auth/register/vendor", request);
        var result = await response.Content.ReadFromJsonAsync<DataResponse<string>>();

        return result ?? new DataResponse<string> { Success = false, Message = "Unknown error" };
    }

    public async Task<AuthResponse> CreateVendorAsync(RequestDTO request)
    {
        var vendorRequest = new RegisterVendorRequest
        {
            Id = request.Id,
            Email = request.Email,
            Password = request.Password,
            DisplayName = request.Name,
            Reason = request.Reason
        };

        var response = await _httpClient.PostAsJsonAsync("api/auth/create/vendor", vendorRequest);
        var result = await response.Content.ReadFromJsonAsync<AuthResponse>();

        // Don't do any of this because account not really created
         if (result.IdToken != null){ 
       
             _currentToken = result.IdToken;
            _currentUserId = result.Data?.Id ?? null;
             _currentUser = result.Data;

             _httpClient.DefaultRequestHeaders.Authorization =
                 new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _currentToken);
         }

        return result ?? new AuthResponse { Success = false, Message = "Unknown error" };
    }

    public async Task<AuthResponse> RegisterAdminAsync(string email, string password) //, string? displayName)
    {
        var request = new RegisterAdminRequest
        {
            Email = email,
            Password = password,
            // DisplayName = displayName
        };

        var response = await _httpClient.PostAsJsonAsync("api/auth/register/admin", request);
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
    public async Task Logout()
    {
        _currentToken = null;
        _currentUserId = null;
        _currentUser = null;
        _httpClient.DefaultRequestHeaders.Authorization = null;

        await _cookies.DeleteCookieAsync("authToken");
        await _cookies.DeleteCookieAsync("userId");
        await _cookies.DeleteCookieAsync("currentUser");
        
        OnAuthStateChanged?.Invoke();
    }

    public async Task<BaseResponse> SendPasswordResetEmailAsync(string email)
    {
        try
        {
            var request = new { Email = email };
            var response = await _httpClient.PostAsJsonAsync("api/auth/password-reset", request);
            var result = await response.Content.ReadFromJsonAsync<BaseResponse>();

            return result ?? new BaseResponse { Success = false, Message = "Unknown error" };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Password reset error: {ex.Message}");
            return new BaseResponse
            {
                Success = false,
                Message = "An error occurred while sending reset email"
            };
        }
    }

    public async Task<BaseResponse> ChangePasswordAsync(string currentPassword, string newPassword)
    {
        try
        {
            if (!IsAuthenticated)
            {
                return new BaseResponse { Success = false, Message = "Not authenticated" };
            }

            var request = new { CurrentPassword = currentPassword, NewPassword = newPassword };
            var response = await _httpClient.PostAsJsonAsync("api/auth/change-password", request);
            var result = await response.Content.ReadFromJsonAsync<BaseResponse>();

            return result ?? new BaseResponse { Success = false, Message = "Unknown error" };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Change password error: {ex.Message}");
            return new BaseResponse
            {
                Success = false,
                Message = "An error occurred while changing password"
            };
        }
    }
}