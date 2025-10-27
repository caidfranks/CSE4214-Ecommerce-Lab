using Google.Cloud.Firestore;

namespace GameVault.Shared.Models;

[FirestoreData]
public class ShoppingCart
{
    [FirestoreProperty] public string CartId { get; set; }
    [FirestoreProperty] public string UserId { get; set; }
    [FirestoreProperty] public List<CartItem> Items { get; set; } = new List<CartItem>();
    public int TotalItemCount => Items?.Sum(i => i.Quantity) ?? 0;

    public ShoppingCart()
    {
    }

    public ShoppingCart(string userId)
    {
        UserId = userId;
    }
}