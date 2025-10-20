using Google.Cloud.Firestore;
using GameVault.Shared.Models;

namespace GameVault.Server.Models.Firestore;

[FirestoreData]
public class FirestoreLog
{
  // File ID
  // [FirestoreProperty]
  // public required string Id { get; set; }

  [FirestoreProperty]
  public string Summary { get; set; } = string.Empty;

  [FirestoreProperty]
  public required LogType Type { get; set; }

  [FirestoreProperty]
  public required string ObjectId { get; set; }

  [FirestoreProperty]
  public required int Status { get; set; }

  [FirestoreProperty]
  public required DateTime Timestamp { get; set; }

  [FirestoreProperty]
  public string Details { get; set; } = string.Empty;
}

[FirestoreData]
public class Log : FirestoreLog, IHasId
{
  public required string Id { get; set; }
}
