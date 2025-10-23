using System.ComponentModel.DataAnnotations;

namespace GameVault.Shared.DTOs;

public class AddToCartDto
{
    [Required] public string ListingId { get; set; } = string.Empty;

    [Required] public int Quantity { get; set; } = 1;
}

