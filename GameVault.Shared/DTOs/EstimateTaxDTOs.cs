using GameVault.Shared.Models;
namespace GameVault.Shared.DTOs;

public class EstimateTaxRequestDTO
{
    public required int SubtotalInCents { get; set; }
    public required string State { get; set; }
}

public class EstimateTaxResponseDTO
{
    public int SubtotalInCents { get; set; }
    public int TaxInCents { get; set; }
    public int TotalInCents { get; set; }
    public decimal TaxRate { get; set; }
}