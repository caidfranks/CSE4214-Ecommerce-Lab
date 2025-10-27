using Google.Cloud.Firestore;
using GameVault.Shared.Models;

namespace GameVault.Shared.Firestore;

[FirestoreData]
public class FirestoreAccount
{
    // File ID
    // [FirestoreProperty]
    // public required string Id { get; set; }

    [FirestoreProperty]
    public required int ApprovalStatus { get; set; }

    //[FirestoreProperty]
    //public Timestamp? ApprovedAt { get; set; } = string.Empty;

    [FirestoreProperty]
    public string ApprovedBy { get; set; } = string.Empty;

    [FirestoreProperty]
    public required string BusinessName { get; set; }

    //[FirestoreProperty]
    //public Timestamp? CreatedAt { get; set; } = string.Empty;

    [FirestoreProperty]
    public required string DisplayName { get; set; }

    [FirestoreProperty]
    public required string Email { get; set; }

    [FirestoreProperty]
    public string RejectionReason { get; set; } = string.Empty;

    [FirestoreProperty]
    public required string Role { get; set; }

    //[FirestoreProperty]
    //public Timestamp? UpdatedAt { get; set; } = string.Empty;
    [FirestoreProperty]
    public required string Id { get; set; }


}

  

