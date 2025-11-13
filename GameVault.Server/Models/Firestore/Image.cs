using Google.Cloud.Firestore;

namespace GameVault.Server.Models.Firestore;

[FirestoreData]
public class FirestoreImage
{
  // File ID
  // [FirestoreProperty]
  // public required string Id { get; set; }

  [FirestoreProperty]
  public required string FileName { get; set; }

  [FirestoreProperty]
  public string AltText { get; set; } = string.Empty;
}

[FirestoreData]
public class Image : FirestoreImage, IHasId
{
  public required string Id { get; set; }
}