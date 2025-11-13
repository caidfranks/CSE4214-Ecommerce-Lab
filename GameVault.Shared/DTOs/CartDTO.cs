namespace GameVault.Shared.DTOs;

public class CartDTO
{
  public string UserId { get; set; } = string.Empty;
  public List<CartItemDTO> Items { get; set; } = new();

  public int SubtotalPriceInCents => Items.Sum(i => i.LineTotalInCents);
  public int SalesTaxInCents => (int)(SubtotalPriceInCents * 0.085m);
  public int TotalPriceInCents => SubtotalPriceInCents + SalesTaxInCents;
  public int TotalItemCount => Items.Sum(i => i.Quantity);
}

public class NewCartItemDTO
{
  public required string ListingId { get; set; }
  public required int Quantity { get; set; }
}

public class CartItemDTO : NewCartItemDTO
{
  public string ListingName { get; set; } = string.Empty;
  public int PriceInCents { get; set; }
  public string ThumbnailUrl { get; set; } = string.Empty;
  public string VendorId { get; set; } = string.Empty;
  public string VendorName { get; set; } = string.Empty;
  public int LineTotalInCents => PriceInCents * Quantity;
}

public class UpdateCartItemQuantityRequest
{
  public int Quantity { get; set; }
}