using GameVault.Shared.Models;
namespace GameVault.Shared.DTOs;

public class UpdateStatusRequestDTO
{
    public required InvoiceStatus Status { get; set; }
}