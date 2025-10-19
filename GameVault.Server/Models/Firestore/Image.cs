using Google.Cloud.Firestore;

namespace GameVault.Server.Models.Firestore;

[FirestoreData]
public class Image
{

  [FirestoreProperty]
  public required string Id { get; set; }

  [FirestoreProperty]
  public required string FileName { get; set; }

  [FirestoreProperty]
  public string AltText { get; set; } = string.Empty;
}