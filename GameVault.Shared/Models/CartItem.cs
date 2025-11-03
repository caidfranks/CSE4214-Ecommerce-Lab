using Google.Cloud.Firestore;

namespace GameVault.Shared.Models;

[FirestoreData]
public class CartItem
{
    [FirestoreProperty] public string ListingId { get; set; }
    [FirestoreProperty] public int Quantity { get; set; }

    public CartItem()
    {
        ListingId = "";
        Quantity = -1;
    }

    public CartItem(string listingId, int quantity)
    {
        ListingId = listingId;
        Quantity = quantity;
    }
}