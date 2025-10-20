// TODO: Update this later
using Google.Cloud.Firestore;

namespace GameVault.Server.Models.Firestore;

[FirestoreData]
public class FirestoreUser
{
  // File ID (& User ID in Auth)
  // [FirestoreProperty]
  // public required string Id { get; set; }

  [FirestoreProperty]
  public required UserRole Type { get; set; }

  [FirestoreProperty]
  public required string Email { get; set; }

  // Password stored via Firebase Auth
}

[FirestoreData]
public class User : FirestoreUser, IHasId
{
  public required string Id { get; set; }
}