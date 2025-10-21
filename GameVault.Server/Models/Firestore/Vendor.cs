// TODO: Update this later
using Google.Cloud.Firestore;

namespace GameVault.Server.Models.Firestore;

[FirestoreData]
public class Vendor : FirestoreUser
{
  [FirestoreProperty]
  public required bool Banned { get; set; }

  [FirestoreProperty]
  public required string Name { get; set; }
}