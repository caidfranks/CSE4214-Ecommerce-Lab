using GameVault.Shared.Models;

namespace GameVault.Shared.DTOs
{
    public class CheckoutRequestDTO
    {
        public required Address ShipTo { get; set; }
        public required string PaymentMethodId { get; set; }
    }

    public class CheckoutResponseDTO
    {
        public bool Success { get; set; }
        public string? OrderId { get; set; }
        public List<string> InvoiceIds { get; set; } = new();
        public int TotalAmountInCents { get; set; }
        public string? ErrorMessage { get; set; }
    }
};