using Google.Cloud.Firestore;

namespace GameVault.Server.Models.Firestore;

public interface IHasId
{
  public string Id { get; set; }
}