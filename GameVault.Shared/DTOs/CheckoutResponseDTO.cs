using GameVault.Shared.Models;

namespace GameVault.Shared.DTOs;

public class CheckoutResponseDTO
{
    public bool Success { get; set; }
    public string? OrderId { get; set; }
    public List<string> InvoiceIds { get; set; } = new();
    public int TotalAmountInCents { get; set; }
    public string? ErrorMessage { get; set; }
}
