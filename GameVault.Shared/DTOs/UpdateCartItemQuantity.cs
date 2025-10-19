using System.ComponentModel.DataAnnotations;

namespace GameVault.Shared.DTOs;

public class UpdateCartItemQuantityDto
{
    [Required]
    public int newQuantity { get; set; }
}