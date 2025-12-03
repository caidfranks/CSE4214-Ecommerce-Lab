namespace GameVault.Server.Models;

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