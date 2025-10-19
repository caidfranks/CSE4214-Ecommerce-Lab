using Google.Cloud.Firestore;

namespace GameVault.Shared.Models;

[FirestoreData]
public class ShoppingCart
{
    [FirestoreProperty]
    public string CartId { get; set; }
    [FirestoreProperty]
    public string UserId { get; set; }
    [FirestoreProperty]
    public List<CartItem> Items { get; set; } = new List<CartItem>();
    [FirestoreProperty]
    public DateTime LastModified { get; set; } = DateTime.UtcNow;
    public int TotalPriceInCents => Items?.Sum(i => i.PriceAtAddTimeInCents * i.Quantity) ?? 0;
    public int TotalItemCount => Items?.Sum(i => i.Quantity) ?? 0;

    public ShoppingCart()
    {
    }

    public ShoppingCart(string userId)
    {
        UserId = userId;
    }
}