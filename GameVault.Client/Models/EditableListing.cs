using System;
using System.Diagnostics.CodeAnalysis;
using GameVault.Shared.DTOs;

namespace GameVault.Client.Models;

public class EditableListing
{
  public required string Name { get; set; }
  public required string Description { get; set; }
  public required decimal Price { get; set; }
  public required int Stock { get; set; }
  public static EditableListing FromListingDTO(ListingDTO dto)
  {
    return new()
    {
      Name = dto.Name,
      Description = dto.Description,
      Price = dto.Price / 100M,
      Stock = dto.Stock
    };
  }
}
