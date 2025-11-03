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
    private readonly IConfiguration _configuration;
    private string? _apiKey;
    private string? _currentUserId;
    private string? _currentToken;
    private UserDTO? _currentUser;

    public AuthService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _apiKey = _configuration["Firebase:ApiKey"];


    }

    public bool IsAuthenticated => !string.IsNullOrEmpty(_currentToken);
    public string? CurrentUserId => _currentUserId;
    public UserDTO? CurrentUser => _currentUser;
    public string? Token => _currentToken;

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

    public async Task<BaseResponse> SendPasswordResetEmailAsync(string email)
    {
        try
        {
            var content = new StringContent(
                JsonSerializer.Serialize(new { email, requestType = "PASSWORD_RESET" }),
                Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.PostAsync(
                $"https://identitytoolkit.googleapis.com/v1/accounts:sendOobCode?key={_apiKey}",
                content
            );

            if (response.IsSuccessStatusCode)
            {
                return new BaseResponse
                {
                    Success = true,
                    Message = "Password reset email sent successfully"
                };
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            var errorData = JsonSerializer.Deserialize<FirebaseErrorResponse>(errorContent);

            return new BaseResponse
            {
                Success = false,
                Message = errorData?.Error?.Message ?? "Failed to send reset email"
            };
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
            if (string.IsNullOrEmpty(_currentToken))
            {
                return new BaseResponse { Success = false, Message = "Not authenticated" };
            }

            // First verify current password by trying to sign in
            var verifyContent = new StringContent(
                JsonSerializer.Serialize(new { email = CurrentUser?.Email, password = currentPassword, returnSecureToken = true }),
                Encoding.UTF8,
                "application/json"
            );

            var verifyResponse = await _httpClient.PostAsync(
                $"https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key={_apiKey}",
                verifyContent
            );

            if (!verifyResponse.IsSuccessStatusCode)
            {
                return new BaseResponse { Success = false, Message = "Current password is incorrect" };
            }

            // Now change the password
            var content = new StringContent(
                JsonSerializer.Serialize(new { idToken = _currentToken, password = newPassword, returnSecureToken = true }),
                Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.PostAsync(
                $"https://identitytoolkit.googleapis.com/v1/accounts:update?key={_apiKey}",
                content
            );

            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadFromJsonAsync<FirebaseAuthResponse>();
                if (responseData != null)
                {
                    _currentToken = responseData.IdToken;
                    // Update the authorization header with new token
                    _httpClient.DefaultRequestHeaders.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _currentToken);
                }

                return new BaseResponse { Success = true, Message = "Password changed successfully" };
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            return new BaseResponse { Success = false, Message = "Failed to change password" };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Change password error: {ex.Message}");
            return new BaseResponse { Success = false, Message = "An error occurred" };
        }
    }
}