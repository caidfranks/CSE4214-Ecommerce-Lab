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