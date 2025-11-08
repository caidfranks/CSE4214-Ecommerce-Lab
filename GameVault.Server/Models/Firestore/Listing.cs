using Google.Cloud.Firestore;
using GameVault.Shared.Models;

namespace GameVault.Server.Models.Firestore;

[FirestoreData]
public class FirestoreListing
{
  // File ID
  // [FirestoreProperty]
  // public required string Id { get; set; }

  [FirestoreProperty]
  public required string Name { get; set; }

  [FirestoreProperty]
  public required int Price { get; set; }

  [FirestoreProperty]
  public string Description { get; set; } = string.Empty;

  [FirestoreProperty]
  public required int Stock { get; set; }

  [FirestoreProperty]
  public required ListingStatus Status { get; set; }

  [FirestoreProperty]
  public required string OwnerID { get; set; }

  [FirestoreProperty]
  public string Image { get; set; } = string.Empty;

  [FirestoreProperty]
  public required DateTime LastModified { get; set; }

  [FirestoreProperty]
  public string RemoveMsg { get; set; } = string.Empty;

  [FirestoreProperty("category")]
  public required string Category { get; set; }
}

[FirestoreData]
public class Listing : FirestoreListing, IHasId
{
  public required string Id { get; set; }
}
