using System;
using Google.Cloud.Firestore;

namespace GameVault.Shared.Models;


[FirestoreData]
public class Listing
{
  public required string Id { get; set; }
  [FirestoreProperty]
  public required string Name { get; set; }
  [FirestoreProperty]
  public required int Price { get; set; }
  [FirestoreProperty]
  public required string Description { get; set; }
  [FirestoreProperty]
  public required int Stock { get; set; }
  [FirestoreProperty]
  public required int Status { get; set; }
  [FirestoreProperty]
  public required string OwnerID { get; set; }
  [FirestoreProperty]
  public required string Image { get; set; }
    [FirestoreProperty]
    public string Category { get; set; } = string.Empty;

}
