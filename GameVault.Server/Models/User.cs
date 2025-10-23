using Google.Cloud.Firestore;

namespace GameVault.Server.Models;

public enum UserRole
{
    Customer,
    Vendor,
    Admin
}

public enum ApprovalStatus
{
    Pending,
    Approved,
    Rejected,
    Suspended
}

[FirestoreData]
public class User
{
    [FirestoreProperty] public string UserId { get; set; } = string.Empty;
    [FirestoreProperty] public string Email { get; set; } = string.Empty;
    [FirestoreProperty] public string? DisplayName { get; set; }
    [FirestoreProperty] public string Role { get; set; } = nameof(UserRole.Customer);
    [FirestoreProperty] public string? ApprovalStatus { get; set; }
    [FirestoreProperty] public string? BusinessName { get; set; }
    [FirestoreProperty]public string? BusinessDescription { get; set; }
    [FirestoreProperty] public string? RejectionReason { get; set; }
}