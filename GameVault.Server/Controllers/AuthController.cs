using Microsoft.AspNetCore.Mvc;
using GameVault.Server.Models;
using GameVault.Server.Services;
using GameVault.Shared.Models;
using GameVault.Shared.DTOs;
using GameVault.Server.Models.Firestore;
using Microsoft.AspNetCore.Identity;
using System.Data;
using System.Text.Json;

namespace GameVault.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    NotificationService _notifService;
    UserService _userService;
    private readonly IFirebaseAuthService _firebaseAuth;
    private readonly IFirestoreService _firestore;
    private readonly IConfiguration _configuration;

    public AuthController(IFirebaseAuthService firebaseAuth, IFirestoreService firestore, IConfiguration configuration, UserService userService, NotificationService notifService)
    {
        _notifService = notifService;
        _userService = userService;
        _firebaseAuth = firebaseAuth;
        _firestore = firestore;
        _configuration = configuration;
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
    {
        var apiKey = _configuration["Firebase:ApiKey"];
        if (string.IsNullOrEmpty(apiKey))
        {
            return StatusCode(500, new AuthResponse
            {
                Success = false,
                Message = "Firebase configuration error"
            });
        }

        using var httpClient = new HttpClient();
        var firebaseAuthUrl = $"https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key={apiKey}";

        string? emulatorHost = Environment.GetEnvironmentVariable("FIREBASE_AUTH_EMULATOR_HOST");
        if (!string.IsNullOrEmpty(emulatorHost))
        {
            firebaseAuthUrl = $"http://{emulatorHost}/identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key={apiKey}";
        }

        var loginData = new
        {
            email = request.Email,
            password = request.Password,
            returnSecureToken = true
        };

        var response = await httpClient.PostAsJsonAsync(firebaseAuthUrl, loginData);

        if (!response.IsSuccessStatusCode)
        {
            return BadRequest(new AuthResponse
            {
                Success = false,
                Message = "Invalid email or password"
            });
        }

        var firebaseResponse = await response.Content.ReadFromJsonAsync<FirebaseLoginResponse>();

        if (firebaseResponse == null || string.IsNullOrEmpty(firebaseResponse.IdToken))
        {
            return BadRequest(new AuthResponse
            {
                Success = false,
                Message = "Login failed"
            });
        }

        var user = await _firestore.GetDocumentAsync<User>("users", firebaseResponse.LocalId);

        if (user == null)
        {
            return NotFound(new AuthResponse
            {
                Success = false,
                Message = "User profile not found"
            });
        }

        if (user.Banned == true)
        {
            return StatusCode(403, new AuthResponse
            {
                Success = false,
                Message = string.IsNullOrEmpty(user.BanMsg)
                    ? "Your account has been banned. Please contact support for more information."
                    : $"Your account has been banned. {user.BanMsg}"
            });
        }

        return Ok(new AuthResponse
        {
            Success = true,
            Message = "Login successful",
            IdToken = firebaseResponse.IdToken,
            Data = new UserDTO
            {
                Id = firebaseResponse.LocalId,
                Email = user.Email,
                Name = user.Name,
                Type = user.Type,
                Banned = user.Banned,
                BanMsg = user.BanMsg,
                ReviewedBy = user.ReviewedBy,
                BalanceInCents = user.BalanceInCents
            }
        });
    }

    [HttpPost("register/customer")]
    public async Task<ActionResult<AuthResponse>> RegisterCustomer([FromBody] RegisterCustomerRequest request)
    {
        try
        {
            var userId = await _firebaseAuth.CreateUserAsync(request.Email, request.Password);

            if (userId is null) throw new Exception("User creation failed");

            var user = new Models.Firestore.User()
            {
                Id = userId,
                Type = AccountType.Customer,
                Email = request.Email,
                Banned = false
            };

            await _firestore.SetDocumentAsync("users", userId, user);

            return Ok(new AuthResponse
            {
                Success = true,
                Message = "Customer account created successfully",
                Data = new UserDTO
                {
                    Id = user.Id,
                    Email = user.Email,
                    Type = user.Type,
                    Banned = false
                }
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new AuthResponse
            {
                Success = false,
                Message = ex.Message
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return StatusCode(500, new AuthResponse
            {
                Success = false,
                Message = "An unexpected error occurred while creating your account. Please try again later."
            });
        }
    }

    [HttpPost("register/vendor")]
    public async Task<ActionResult<DataResponse<string>>> RegisterVendor([FromBody] RegisterVendorRequest request)
    {
        try
        {
            var application = new FirestoreRequest
            {
                Email = request.Email,
                Password = request.Password,
                Name = request.DisplayName,
                Reason = request.Reason,
                Timestamp = DateTime.UtcNow,
                Archived = false
            };

            var id = await _firestore.AddDocumentAsync("requests", application);

            if (id is not null)
            {
                var adminUsers = await _firestore.QueryDocumentsAsyncWithId<User>("users", "Type", (int)AccountType.Admin);
                foreach (var admin in adminUsers)
                {
                    await _notifService.CreateNotifAsync(admin.Id, "New Vendor Application", "A new vendor application has been submitted");
                }

                return new DataResponse<string>
                {
                    Success = true,
                    Message = "Your application has been submitted",
                    Data = id.Id
                };
            }
            else
            {
                return new DataResponse<string>
                {
                    Success = false,
                    Message = "Failed to submit application. Please try again",
                };
            }
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new DataResponse<string>
            {
                Success = false,
                Message = ex.Message
            });
        }
        catch (Exception)
        {
            return StatusCode(500, new DataResponse<string>
            {
                Success = false,
                Message = "An unexpected error occurred while creating your vendor account. Please try again later."
            });
        }
    }

    [HttpPost("create/vendor")]
    public async Task<ActionResult<AuthResponse>> CreateVendor([FromBody] RegisterVendorRequest request, [FromHeader] string? Authorization)

    {
        try
        {
            //var userId = await _firebaseAuth.CreateUserAsync(request.Email, request.Password);
            var userId = request.Id;
            var currentUserId = await _userService.GetUserFromHeader(Authorization);

            var user = new User
            {
                Id = userId!,
                BanMsg = null,
                Banned = false,
                Email = request.Email,
                Name = request.DisplayName ?? string.Empty,
                Type = AccountType.Vendor,
                ReviewedBy = currentUserId.Id
            };

            await _firestore.SetDocumentAsync("users", userId!, user);
            await _notifService.CreateNotifAsync(userId, "Vendor Application Approved", "Your vendor account has been approved. You may now login.");

            return Ok(new AuthResponse
            {
                Success = true,
                Message = "Vendor account successfully created.",
                Data = new UserDTO
                {
                    Id = userId,
                    Email = user.Email,
                    Name = user.Name,
                    Type = user.Type,
                    Banned = user.Banned,
                    BanMsg = user.BanMsg,
                    ReviewedBy = user.ReviewedBy
                }
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new DataResponse<string>
            {
                Success = false,
                Message = ex.Message
            });
        }
        catch (Exception)
        {
            return StatusCode(500, new DataResponse<string>
            {
                Success = false,
                Message = "An unexpected error occurred while creating your vendor account. Please try again later."
            });
        }
    }

    [HttpPost("deny/vendor")]
    public async Task<ActionResult<AuthResponse>> CreateDeniedVendor([FromBody] RegisterVendorRequest request, [FromHeader] string? Authorization)

    {
        try
        {
            //var userId = await _firebaseAuth.CreateUserAsync(request.Email, request.Password);
            var userId = request.Id;
            var currentUserId = await _userService.GetUserFromHeader(Authorization);

            var user = new User
            {
                Id = userId!,
                BanMsg = request.Reason,
                Banned = true,
                Email = request.Email,
                Name = request.DisplayName ?? string.Empty,
                Type = AccountType.Vendor,
                ReviewedBy = currentUserId.Id
            };

            await _firestore.SetDocumentAsync("users", userId!, user);
            await _notifService.CreateNotifAsync(userId, "Vendor Application Denied", $"Your vendor account has been Denied. Reason: {request.Reason}.");

            return Ok(new AuthResponse
            {
                Success = true,
                Message = "Vendor account denied.",
                Data = new UserDTO
                {
                    Id = userId,
                    Email = user.Email,
                    Name = user.Name,
                    Type = user.Type,
                    Banned = user.Banned,
                    BanMsg = user.BanMsg,
                    ReviewedBy = user.ReviewedBy
                }
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new AuthResponse
            {
                Success = false,
                Message = ex.Message
            });
        }
        catch (Exception)
        {
            return StatusCode(500, new AuthResponse
            {
                Success = false,
                Message = "An unexpected error occurred while creating your vendor account. Please try again later."
            });
        }
    }

    [HttpPost("register/admin")]
    public async Task<ActionResult<AuthResponse>> RegisterAdmin([FromBody] RegisterAdminRequest request)
    {
        try
        {
            var userId = await _firebaseAuth.CreateUserAsync(request.Email, request.Password);

            if (userId is null) throw new Exception("User creation failed");

            var user = new Models.Firestore.User()
            {
                Id = userId,
                Type = AccountType.Admin,
                Email = request.Email,
                //Name = request.DisplayName,
                Banned = null
            };

            await _firestore.SetDocumentAsync("users", userId, user);

            return Ok(new AuthResponse
            {
                Success = true,
                Message = "Customer account created successfully",
                Data = new UserDTO
                {
                    Id = user.Id,
                    Email = user.Email,
                    Type = user.Type,
                    //Name = request.DisplayName,
                    Banned = null
                }
                // No ID Token because not actually logging in
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new AuthResponse
            {
                Success = false,
                Message = ex.Message
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return StatusCode(500, new AuthResponse
            {
                Success = false,
                Message = "An unexpected error occurred while creating your account. Please try again later."
            });
        }
    }

    [HttpGet("verify")]
    public async Task<ActionResult<AuthResponse>> VerifyToken([FromHeader] string? Authorization)
    {
        if (Authorization is null) return Unauthorized(new AuthResponse
        {
            Success = false,
            Message = "No token"
        });

        string token = Authorization.Split(" ").ToList()[1]; // Extract 2nd part of header formatted "Bearer [___Token___]"
        var userId = await _firebaseAuth.VerifyTokenAsync(token);

        if (string.IsNullOrEmpty(userId))
        {
            Console.WriteLine("Attempted access with expired token");
            return Unauthorized(new AuthResponse
            {
                Success = false,
                Message = "Invalid or expired token"
            });
        }

        var user = await _firestore.GetDocumentAsync<FirestoreUser>("users", userId);

        if (user == null)
        {
            return NotFound(new AuthResponse
            {
                Success = false,
                Message = "User profile not found"
            });
        }

        return Ok(new AuthResponse
        {
            Success = true,
            Message = "Login successful",
            Data = new UserDTO
            {
                Id = userId,
                Email = user.Email,
                Name = user.Name,
                Type = user.Type,
                Banned = user.Banned,
                BanMsg = user.BanMsg,
                ReviewedBy = user.ReviewedBy,
            }
        });
    }

    [HttpPost("password-reset")]
    public async Task<ActionResult<BaseResponse>> SendPasswordReset([FromBody] PasswordResetRequest request)
    {
        try
        {
            var apiKey = _configuration["Firebase:ApiKey"];
            if (string.IsNullOrEmpty(apiKey))
            {
                return StatusCode(500, new BaseResponse
                {
                    Success = false,
                    Message = "Firebase configuration error"
                });
            }

            using var httpClient = new HttpClient();
            var firebaseAuthUrl = $"https://identitytoolkit.googleapis.com/v1/accounts:sendOobCode?key={apiKey}";

            string? emulatorHost = Environment.GetEnvironmentVariable("FIREBASE_AUTH_EMULATOR_HOST");
            if (!string.IsNullOrEmpty(emulatorHost))
            {
                firebaseAuthUrl = $"http://{emulatorHost}/identitytoolkit.googleapis.com/v1/accounts:sendOobCode?key={apiKey}";
            }

            var resetData = new
            {
                email = request.Email,
                requestType = "PASSWORD_RESET"
            };

            var response = await httpClient.PostAsJsonAsync(firebaseAuthUrl, resetData);

            if (response.IsSuccessStatusCode)
            {
                return Ok(new BaseResponse
                {
                    Success = true,
                    Message = "Password reset email sent successfully"
                });
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            var errorData = JsonSerializer.Deserialize<FirebaseErrorResponse>(errorContent);

            return BadRequest(new BaseResponse
            {
                Success = false,
                Message = errorData?.Error?.Message ?? "Failed to send reset email"
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Password reset error: {ex.Message}");
            return StatusCode(500, new BaseResponse
            {
                Success = false,
                Message = "An error occurred while sending reset email"
            });
        }
    }

    [HttpPost("change-password")]
    public async Task<ActionResult<BaseResponse>> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        try
        {
            var apiKey = _configuration["Firebase:ApiKey"];
            if (string.IsNullOrEmpty(apiKey))
            {
                return StatusCode(500, new BaseResponse
                {
                    Success = false,
                    Message = "Firebase configuration error"
                });
            }

            // Get the token from Authorization header
            var authHeader = Request.Headers["Authorization"].ToString();
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                return Unauthorized(new BaseResponse
                {
                    Success = false,
                    Message = "Not authenticated"
                });
            }

            var token = authHeader.Substring("Bearer ".Length);

            // Verify the token and get user info
            var userId = await _firebaseAuth.VerifyTokenAsync(token);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new BaseResponse
                {
                    Success = false,
                    Message = "Invalid or expired token"
                });
            }

            var user = await _firestore.GetDocumentAsync<FirestoreUser>("users", userId);
            if (user == null)
            {
                return NotFound(new BaseResponse
                {
                    Success = false,
                    Message = "User not found"
                });
            }

            using var httpClient = new HttpClient();
            var firebaseAuthUrl = $"https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key={apiKey}";

            string? emulatorHost = Environment.GetEnvironmentVariable("FIREBASE_AUTH_EMULATOR_HOST");
            if (!string.IsNullOrEmpty(emulatorHost))
            {
                firebaseAuthUrl = $"http://{emulatorHost}/identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key={apiKey}";
            }

            // Verify current password
            var verifyData = new
            {
                email = user.Email,
                password = request.CurrentPassword,
                returnSecureToken = true
            };

            var verifyResponse = await httpClient.PostAsJsonAsync(firebaseAuthUrl, verifyData);

            if (!verifyResponse.IsSuccessStatusCode)
            {
                return BadRequest(new BaseResponse
                {
                    Success = false,
                    Message = "Current password is incorrect"
                });
            }

            // Update password
            var updateUrl = $"https://identitytoolkit.googleapis.com/v1/accounts:update?key={apiKey}";
            if (!string.IsNullOrEmpty(emulatorHost))
            {
                updateUrl = $"http://{emulatorHost}/identitytoolkit.googleapis.com/v1/accounts:update?key={apiKey}";
            }

            var updateData = new
            {
                idToken = token,
                password = request.NewPassword,
                returnSecureToken = true
            };

            var updateResponse = await httpClient.PostAsJsonAsync(updateUrl, updateData);

            if (updateResponse.IsSuccessStatusCode)
            {
                return Ok(new BaseResponse
                {
                    Success = true,
                    Message = "Password changed successfully"
                });
            }

            var errorContent = await updateResponse.Content.ReadAsStringAsync();
            return BadRequest(new BaseResponse
            {
                Success = false,
                Message = "Failed to change password"
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Change password error: {ex.Message}");
            return StatusCode(500, new BaseResponse
            {
                Success = false,
                Message = "An error occurred while changing password"
            });
        }
    }
}

// Request/Response models
public class LoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class PasswordResetRequest
{
    public string Email { get; set; } = string.Empty;
}

public class ChangePasswordRequest
{
    public string CurrentPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}

public class FirebaseLoginResponse
{
    public string IdToken { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public string ExpiresIn { get; set; } = string.Empty;
    public string LocalId { get; set; } = string.Empty;
    public bool Registered { get; set; }
}

public class FirebaseErrorResponse
{
    public FirebaseError? Error { get; set; }
}

public class FirebaseError
{
    public int Code { get; set; }
    public string? Message { get; set; }
}