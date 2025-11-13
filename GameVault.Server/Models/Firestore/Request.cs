using Google.Cloud.Firestore;
using GameVault.Shared.Models;

namespace GameVault.Server.Models.Firestore;

[FirestoreData]
public class FirestoreRequest
{
    // File ID
    // [FirestoreProperty]
    // public required string Id { get; set; }

    [FirestoreProperty]
    public required string Email { get; set; }

    [FirestoreProperty]
    public required string Password { get; set; }

    [FirestoreProperty]
    public required string Name { get; set; }

    [FirestoreProperty]
    public required string Reason { get; set; }

    [FirestoreProperty]
    public string? ReviewedBy { get; set; }

    [FirestoreProperty]
    public required DateTime Timestamp { get; set; }

    [FirestoreProperty]
    public required bool Archived { get; set; }
}

[FirestoreData]
public class Request : FirestoreRequest, IHasId
{
    public required string Id { get; set; }
}