namespace GameVault.Shared.Models;

public class ShoppingCart
{
    public string CartId { get; set; }
    public string UserId { get; set; }
    public List<CartItem> Items { get; set; } = new List<CartItem>();
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