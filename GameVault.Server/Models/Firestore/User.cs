// TODO: Update this later
using GameVault.Shared.Models;
using Google.Cloud.Firestore;

namespace GameVault.Server.Models.Firestore;

[FirestoreData]
public class FirestoreUser
{
  // File ID (& User ID in Auth)
  // [FirestoreProperty]
  // public required string Id { get; set; }

  [FirestoreProperty]
  public required AccountType Type { get; set; }

  [FirestoreProperty]
  public required string Email { get; set; }

  [FirestoreProperty]
  public bool? Banned { get; set; }

  [FirestoreProperty]
  public string? BanMsg { get; set; }

  [FirestoreProperty]
  public string? Name { get; set; }

  [FirestoreProperty]
  public string? ReviewedBy { get; set; }

    // Password stored via Firebase Auth
    [FirestoreProperty]
    public string SquareCustomerId { get; set; } = string.Empty;

    [FirestoreProperty]
    public string SquareCardId { get; set; } = string.Empty;
}

[FirestoreData]
public class User : FirestoreUser, IHasId
{
  public required string Id { get; set; }
}