using Microsoft.AspNetCore.Mvc;
using GameVault.Server.Models;
using GameVault.Server.Services;

namespace GameVault.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IFirebaseAuthService _firebaseAuth;
    private readonly IFirestoreService _firestore;
    private readonly IConfiguration _configuration;

    public AuthController(IFirebaseAuthService firebaseAuth, IFirestoreService firestore, IConfiguration configuration)
    {
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

        return Ok(new AuthResponse
        {
            Success = true,
            Message = "Login successful",
            UserId = firebaseResponse.LocalId,
            IdToken = firebaseResponse.IdToken,
            User = new UserProfile
            {
                UserId = user.UserId ?? string.Empty,
                Email = user.Email ?? string.Empty,
                DisplayName = user.DisplayName,
                Role = user.Role ?? string.Empty,
                ApprovalStatus = user.ApprovalStatus,
                BusinessName = user.BusinessName,
                CreatedAt = user.CreatedAt
            }
        });
    }
    
    [HttpPost("register/customer")]
    public async Task<ActionResult<AuthResponse>> RegisterCustomer([FromBody] RegisterCustomerRequest request)
    {
        var userId = await _firebaseAuth.CreateUserAsync(request.Email, request.Password);

        var user = new User
        {
            UserId = userId!,
            Email = request.Email,
            DisplayName = request.DisplayName ?? string.Empty,
            Role = nameof(UserRole.Customer),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _firestore.SetDocumentAsync("users", userId!, user);

        return Ok(new AuthResponse
        {
            Success = true,
            Message = "Customer account created successfully",
            UserId = userId,
            User = new UserProfile
            {
                UserId = user.UserId,
                Email = user.Email,
                DisplayName = user.DisplayName,
                Role = user.Role,
                CreatedAt = user.CreatedAt
            }
        });
    }

    [HttpPost("register/vendor")]
    public async Task<ActionResult<AuthResponse>> RegisterVendor([FromBody] RegisterVendorRequest request)
    {
        var userId = await _firebaseAuth.CreateUserAsync(request.Email, request.Password);
        
        var user = new User
        {
            UserId = userId!,
            Email = request.Email,
            DisplayName = request.DisplayName ?? string.Empty,
            Role = nameof(UserRole.Vendor),
            ApprovalStatus = nameof(ApprovalStatus.Pending),
            BusinessName = request.BusinessName,
            BusinessDescription = request.BusinessDescription,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _firestore.SetDocumentAsync("users", userId!, user);

        return Ok(new AuthResponse
        {
            Success = true,
            Message = "Vendor application submitted. Please wait for admin approval.",
            UserId = userId,
            User = new UserProfile
            {
                UserId = user.UserId,
                Email = user.Email,
                DisplayName = user.DisplayName,
                Role = user.Role,
                ApprovalStatus = user.ApprovalStatus,
                BusinessName = user.BusinessName,
                BusinessDescription = user.BusinessDescription,
                CreatedAt = user.CreatedAt
            }
        });
    }

    [HttpPost("verify")]
    public async Task<ActionResult<AuthResponse>> VerifyToken([FromBody] VerifyTokenRequest request)
    {
        var userId = await _firebaseAuth.VerifyTokenAsync(request.IdToken);

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new AuthResponse
            {
                Success = false,
                Message = "Invalid or expired token"
            });
        }
        
        var user = await _firestore.GetDocumentAsync<User>("users", userId);

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
            Message = "Token verified",
            UserId = userId,
            User = new UserProfile
            {
                UserId = user.UserId ?? string.Empty,
                Email = user.Email ?? string.Empty,
                DisplayName = user.DisplayName ?? string.Empty,
                Role = user.Role ?? string.Empty,
                ApprovalStatus = user.ApprovalStatus,
                BusinessName = user.BusinessName,
                CreatedAt = user.CreatedAt
            }
        });
    }
}

public class LoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
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