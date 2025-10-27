namespace GameVault.Server.Models;

public class RegisterCustomerRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? DisplayName { get; set; }
}

public class RegisterVendorRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string BusinessName { get; set; } = string.Empty;
    public string BusinessDescription { get; set; } = string.Empty;
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
    public string? BusinessDescription { get; set; }
}

public class VerifyTokenRequest
{
    public string IdToken { get; set; } = string.Empty;
}

public class VendorApprovalRequest
{
    public string VendorUserId { get; set; } = string.Empty;
    public bool Approved { get; set; }
    public string? RejectionReason { get; set; }
}

public class VendorApplicationResponse
{
    public string Status { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? RejectionReason { get; set; }
}