using GameVault.Shared.Models;
namespace GameVault.Shared.DTOs;

public class OrderDTO
{
    public string Id { get; set; } = string.Empty;
    public string CustomerId { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public int SubtotalInCents { get; set; }
    public int TaxInCents { get; set; }
    public int TotalInCents { get; set; }
}