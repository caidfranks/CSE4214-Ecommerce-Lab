// TODO: Update this later
using Google.Cloud.Firestore;

namespace GameVault.Server.Models.Firestore;

[FirestoreData]
public class Admin : FirestoreUser
{
  [FirestoreProperty]
  public required bool Banned { get; set; }
}