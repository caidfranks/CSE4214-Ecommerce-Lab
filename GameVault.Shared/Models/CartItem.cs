using Google.Cloud.Firestore;

namespace GameVault.Shared.Models;

[FirestoreData]
public class CartItem
{
    [FirestoreProperty] public string CartItemId { get; set; }
    [FirestoreProperty] public string ListingId { get; set; }
    [FirestoreProperty] public string ThumbnailUrl { get; set; }
    [FirestoreProperty] public string ListingName { get; set; }
    [FirestoreProperty] public int PriceAtAddTimeInCents { get; set; }
    [FirestoreProperty] public int Quantity { get; set; }
    [FirestoreProperty] public string VendorId { get; set; }
    [FirestoreProperty] public string VendorName { get; set; }
    

    public CartItem()
    {
    }

    public CartItem(string listingId, string thumbnailUrl, string listingName, int priceInCents, int quantity, string vendorId, string vendorName)
    {
        ListingId = listingId;
        ThumbnailUrl = thumbnailUrl;
        ListingName = listingName;
        PriceAtAddTimeInCents = priceInCents;
        Quantity = quantity;
        VendorId = vendorId;
        VendorName = vendorName;
        
    }
}