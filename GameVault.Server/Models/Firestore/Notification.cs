using Google.Cloud.Firestore;
using GameVault.Shared.Models;

namespace GameVault.Server.Models.Firestore;

[FirestoreData]
public class FirestoreNotification
{
  // File ID
  // [FirestoreProperty]
  // public required string Id { get; set; }

  [FirestoreProperty] public required DateTime Timestamp { get; set; }

  [FirestoreProperty] public required string UserId { get; set; }
  [FirestoreProperty] public required string Title { get; set; }
  [FirestoreProperty] public required string Message { get; set; }
}

[FirestoreData]
public class Notification : FirestoreNotification, IHasId
{
  public required string Id { get; set; }
}