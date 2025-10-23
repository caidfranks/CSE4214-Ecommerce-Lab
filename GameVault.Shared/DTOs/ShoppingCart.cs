namespace GameVault.Shared.DTOs;

public class ShoppingCartDto
{
    public string UserId { get; set; } = string.Empty;
    public List<CartItemDto> Items { get; set; } = new();
    
    public int SubtotalPriceInCents => Items.Sum(i => i.LineTotalInCents);
    public int SalesTaxInCents => (int)(SubtotalPriceInCents * 0.085m);
    public int TotalPriceInCents => SubtotalPriceInCents + SalesTaxInCents;
    public int TotalItemCount => Items.Sum(i => i.Quantity);
}