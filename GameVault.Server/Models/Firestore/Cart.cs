using Google.Cloud.Firestore;

namespace GameVault.Server.Models.Firestore;

[FirestoreData]
public class FirestoreCart
{
  // File ID
  // [FirestoreProperty]
  // public required string OwnerId { get; set; }

  [FirestoreProperty]
  public required List<CartItem> Items {get; set;}
}

[FirestoreData]
public class CartWithId : FirestoreCart, IHasId
{
  public required string Id { get; set; }
}

public class Cart : FirestoreCart
{
  public required string OwnerId { get; set; }

  public static Cart FromCartWithId(CartWithId oldCart)
  {
    return new()
    {
      OwnerId = oldCart.Id,
      Items = oldCart.Items
    };
  }
}